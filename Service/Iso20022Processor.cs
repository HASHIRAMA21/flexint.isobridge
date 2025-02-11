using System.Data;
using System.Data.SqlClient;
using FlexInt.ISOBridge.Data;
using ISO20022;
using Microsoft.Extensions.Logging;

namespace FlexInt.ISOBridge.Service;

public class Iso20022Processor : IMessageProcessor
{
    private readonly DatabaseManager _databaseManager;
    private readonly IParsingService _parsingService;
    private readonly ILogger<Iso20022Processor> _logger;

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
            await _parsingService.ParseXmlAsync(xmlFilePath, InsertDataAsync);
            _logger.LogInformation($"Finished processing of file: {xmlFilePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing file {xmlFilePath}");
            throw;
        }
    }

    private async Task InsertDataAsync(DataSet dataSet, Guid messageGuid, Guid fileGuid)
    {
        using var connection = _databaseManager.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            string messageType = GetMessageType(dataSet);
            await InsertMessageMetadataAsync(connection, transaction, messageGuid, messageType);

            foreach (DataTable dataTable in dataSet.Tables)
            {
                string tableName = dataTable.TableName;
                await CreateTableIfNotExistsAsync(connection, transaction, tableName, dataTable.Columns);

                string insertQuery = BuildInsertQuery(dataTable);

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

    private string BuildInsertQuery(DataTable dataTable)
    {
        var columns = new List<string>();
        var values = new List<string> { "@MessageGuid" };

        foreach (DataColumn column in dataTable.Columns)
        {
            if (column.ColumnName != "MessageGuid" && column.ColumnName != "Message")
            {
                columns.Add(column.ColumnName);
                values.Add($"@{column.ColumnName}");
            }
        }

        return $"INSERT INTO {dataTable.TableName} (MessageGuid, {string.Join(", ", columns)}) VALUES ({string.Join(", ", values)});";
    }

    private void AddRowParameters(SqlCommand command, DataColumnCollection columns, DataRow row)
    {
        foreach (DataColumn column in columns)
        {
            if (column.ColumnName != "MessageGuid" && column.ColumnName != "Message")
            {
                command.Parameters.AddWithValue($"@{column.ColumnName}", row[column.ColumnName] ?? DBNull.Value);
            }
        }
    }

    private async Task InsertMessageMetadataAsync(SqlConnection connection, SqlTransaction transaction, Guid messageGuid, string messageType)
    {
        const string insertMessageQuery = @"
            INSERT INTO Messages (MessageGuid, MessageId, MessageType, CreationDateTime, OriginalMessageId, OriginalMessageNameId, Status, AdditionalInfo)
            VALUES (@MessageGuid, @MessageId, @MessageType, @CreationDateTime, @OriginalMessageId, @OriginalMessageNameId, @Status, @AdditionalInfo);";

        using var command = new SqlCommand(insertMessageQuery, connection, transaction);
        command.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier).Value = messageGuid;
        command.Parameters.Add("@MessageId", SqlDbType.VarChar, 255).Value = DBNull.Value;
        command.Parameters.Add("@MessageType", SqlDbType.VarChar, 255).Value = messageType;
        command.Parameters.Add("@CreationDateTime", SqlDbType.DateTime).Value = DateTime.UtcNow;
        command.Parameters.Add("@OriginalMessageId", SqlDbType.VarChar, 255).Value = DBNull.Value;
        command.Parameters.Add("@OriginalMessageNameId", SqlDbType.VarChar, 255).Value = DBNull.Value;
        command.Parameters.Add("@Status", SqlDbType.VarChar, 255).Value = "Processed";
        command.Parameters.Add("@AdditionalInfo", SqlDbType.VarChar).Value = DBNull.Value;
        await command.ExecuteNonQueryAsync();
    }

    private string GetMessageType(DataSet dataSet)
    {
        if (dataSet.Tables.Contains("Messages") && dataSet.Tables["Messages"].Rows.Count > 0)
        {
            DataRow messageRow = dataSet.Tables["Messages"].Rows[0];
            if (messageRow.Table.Columns.Contains("MessageType"))
            {
                return messageRow["MessageType"].ToString();
            }
        }
        return "UnknownMessageType";
    }

    private async Task CreateTableIfNotExistsAsync(SqlConnection connection, SqlTransaction transaction, string tableName, DataColumnCollection columns)
    {
        using var command = new SqlCommand($"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'", connection, transaction);
        if (await command.ExecuteScalarAsync() == null)
        {
            var createTableQuery = new List<string> { "MessageGuid UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Messages(MessageGuid)", "Id BIGINT IDENTITY(1,1) PRIMARY KEY" };

            foreach (DataColumn column in columns)
            {
                if (column.ColumnName != "MessageGuid" && column.ColumnName != "Message")
                {
                    createTableQuery.Add($"{column.ColumnName} {GetSqlType(column.DataType)}");
                }
            }

            await new SqlCommand($"CREATE TABLE {tableName} ({string.Join(", ", createTableQuery)});", connection, transaction).ExecuteNonQueryAsync();
        }
    }

    private string GetSqlType(Type type)
    {
        return type.Name switch
        {
            "Int32" => "INT",
            "Decimal" => "DECIMAL(18,2)",
            "DateTime" => "DATETIME2",
            "Boolean" => "BIT",
            "String" => "TEXT",
            _ => "VARCHAR(255)"
        };
    }
}