using System.Data;
using System.Data.SqlClient;
using System.Xml;
using FlexInt.ISOBridge.Data;
using ISO20022;
using Microsoft.Extensions.Logging;

namespace FlexInt.ISOBridge.Service;

public class Iso20022Processor : IMessageProcessor
{
    private readonly DatabaseManager _databaseManager;
    private readonly IParsingService _parsingService;
    private readonly ILogger<Iso20022Processor> _logger;

    private static readonly Dictionary<string, string> NamespaceToTableMap = new()
    {
        { "urn:iso:std:iso:20022:tech:xsd:pacs.003.001.05", "pacs_003_001_05" },
        { "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.05", "pacs_008_001_05" },
        { "urn:iso:std:iso:20022:tech:xsd:camt.029.001.05", "camt_029_001_05" },
        { "urn:iso:std:iso:20022:tech:xsd:pacs.004.001.05", "pacs_004_001_05" },
        { "urn:iso:std:iso:20022:tech:xsd:pacs.002.001.06", "pacs_002_001_06" },
        { "urn:iso:std:iso:20022:tech:xsd:pacs.007.001.05", "pacs_007_001_05" }
    };

    public Iso20022Processor(DatabaseManager databaseManager, IParsingService parsingService, ILogger<Iso20022Processor> logger)
    {
        _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        _parsingService = parsingService ?? throw new ArgumentNullException(nameof(parsingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ProcessAsync(string xmlFilePath)
    {
        _logger.LogInformation($"Starting processing of file: {xmlFilePath}");

        try
        {
            string messageType = GetMessageTypeFromXml(xmlFilePath);
            if (!NamespaceToTableMap.TryGetValue(messageType, out string tableName))
            {
                _logger.LogError($"Unknown message type: {messageType}");
                return;
            }

            await _parsingService.ParseXmlAsync(xmlFilePath, (dataSet, messageGuid, fileGuid) => InsertDataAsync(dataSet, messageGuid, fileGuid, tableName));
            _logger.LogInformation($"Finished processing of file: {xmlFilePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing file {xmlFilePath}");
            throw;
        }
    }

    private async Task InsertDataAsync(DataSet dataSet, Guid messageGuid, Guid fileGuid, string tableName)
    {
        using var connection = _databaseManager.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            await InsertMessageMetadataAsync(connection, transaction, messageGuid, tableName);

            foreach (DataTable dataTable in dataSet.Tables)
            {
                await CreateTableIfNotExistsAsync(connection, transaction, tableName, dataTable.Columns);
                string insertQuery = BuildInsertQuery(tableName, dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    using var command = new SqlCommand(insertQuery, connection, transaction);
                    command.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier).Value = messageGuid;
                    AddRowParameters(command, dataTable.Columns, row);
                    await command.ExecuteNonQueryAsync();
                }
            }
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Database transaction failed");
            throw;
        }
    }

    private static string GetMessageTypeFromXml(string xmlFilePath)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlFilePath);
        XmlElement? root = xmlDoc.DocumentElement;
        return root?.NamespaceURI ?? "Unknown";
    }

    private async Task InsertMessageMetadataAsync(SqlConnection connection, SqlTransaction transaction, Guid messageGuid, string tableName)
    {
        const string insertQuery = "INSERT INTO Messages (MessageGuid, MessageType, CreationDateTime, Status) VALUES (@MessageGuid, @MessageType, @CreationDateTime, @Status);";
        using var command = new SqlCommand(insertQuery, connection, transaction);
        command.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier).Value = messageGuid;
        command.Parameters.Add("@MessageType", SqlDbType.VarChar, 255).Value = tableName;
        command.Parameters.Add("@CreationDateTime", SqlDbType.DateTime).Value = DateTime.UtcNow;
        command.Parameters.Add("@Status", SqlDbType.VarChar, 255).Value = "Processed";
        await command.ExecuteNonQueryAsync();
    }

    private async Task CreateTableIfNotExistsAsync(SqlConnection connection, SqlTransaction transaction, string tableName, DataColumnCollection columns)
    {
        using var command = new SqlCommand($"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'", connection, transaction);
        if (await command.ExecuteScalarAsync() == null)
        {
            var createTableQuery = new List<string> { "MessageGuid UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Messages(MessageGuid)", "Id BIGINT IDENTITY(1,1) PRIMARY KEY" };
            foreach (DataColumn column in columns)
            {
                createTableQuery.Add($"{column.ColumnName} {GetSqlType(column.DataType)}");
            }
            await new SqlCommand($"CREATE TABLE {tableName} ({string.Join(", ", createTableQuery)});", connection, transaction).ExecuteNonQueryAsync();
        }
    }

    private static string BuildInsertQuery(string tableName, DataTable dataTable)
    {
        var columns = new List<string>();
        var values = new List<string> { "@MessageGuid" };
        foreach (DataColumn column in dataTable.Columns)
        {
            columns.Add(column.ColumnName);
            values.Add($"@{column.ColumnName}");
        }
        return $"INSERT INTO {tableName} (MessageGuid, {string.Join(", ", columns)}) VALUES ({string.Join(", ", values)});";
    }

    private static void AddRowParameters(SqlCommand command, DataColumnCollection columns, DataRow row)
    {
        foreach (DataColumn column in columns)
        {
            command.Parameters.AddWithValue($"@{column.ColumnName}", row[column.ColumnName] ?? DBNull.Value);
        }
    }

    private static string GetSqlType(Type type) => type.Name switch
    {
        "Int32" => "INT",
        "Decimal" => "DECIMAL(18,2)",
        "DateTime" => "DATETIME2",
        "Boolean" => "BIT",
        "String" => "TEXT",
        _ => "VARCHAR(255)"
    };
}
