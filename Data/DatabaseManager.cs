using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FlexInt.ISOBridge.Data;

public class DatabaseManager
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseManager> _logger;

    public DatabaseManager(string connectionString, ILogger<DatabaseManager> logger)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        InitializeTablesAsync().GetAwaiter().GetResult();
    }

    public SqlConnection CreateConnection() => new SqlConnection(_connectionString);

    private async Task InitializeTablesAsync()
    {
        var processedFilesColumns = new Dictionary<string, string>
        {
            { "FilePath", "NVARCHAR(MAX)" },
            { "ProcessedDate", "DATETIME" }
        };
        await CreateTableIfNotExistsAsync("ProcessedFiles", processedFilesColumns);

        var failedFilesColumns = new Dictionary<string, string>
        {
            { "FilePath", "NVARCHAR(MAX)" },
            { "ErrorMessage", "NVARCHAR(MAX)" },
            { "FailedDate", "DATETIME" }
        };
        await CreateTableIfNotExistsAsync("FailedFiles", failedFilesColumns);
    }
    
    public async Task CreateTableIfNotExistsAsync(string tableName, Dictionary<string, string> columns)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        // Ensure 'Id' is not included in the columns dictionary
        var filteredColumns = columns
            .Where(kvp => kvp.Key != "Id")
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var commandText = $"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{tableName}' AND xtype='U') " +
                          $"CREATE TABLE {tableName} (Id INT PRIMARY KEY IDENTITY, " +
                          string.Join(", ", filteredColumns.Select(c => $"{c.Key} {c.Value}")) + ")";

        using var command = new SqlCommand(commandText, connection);
        await command.ExecuteNonQueryAsync();
    }
    public async Task InsertDataAsync(string tableName, Dictionary<string, object> data)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        // Exclude the 'Id' column from the data dictionary
        var filteredData = data
            .Where(kvp => kvp.Key != "Id")
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var columns = string.Join(", ", filteredData.Keys);
        var values = string.Join(", ", filteredData.Keys.Select(k => $"@{k}"));

        var commandText = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

        using var command = new SqlCommand(commandText, connection);
        foreach (var item in filteredData)
        {
            command.Parameters.AddWithValue($"@{item.Key}", item.Value);
        }

        await command.ExecuteNonQueryAsync();
    }

    public async Task LogProcessedFileAsync(string filePath, string tableName)
    {
        var data = new Dictionary<string, object>
        {
            { "FilePath", filePath },
            { "ProcessedDate", DateTime.UtcNow }
        };

        await InsertDataAsync(tableName, data);
    }

    public async Task LogFailedFileAsync(string filePath, string errorMessage, string tableName)
    {
        var data = new Dictionary<string, object>
        {
            { "FilePath", filePath },
            { "ErrorMessage", errorMessage },
            { "FailedDate", DateTime.UtcNow }
        };

        await InsertDataAsync(tableName, data);
    }

    public async Task<bool> IsFileProcessedAsync(string filePath, string tableName)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var commandText = $"SELECT COUNT(*) FROM {tableName} WHERE FilePath = @FilePath";

        using var command = new SqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@FilePath", filePath);
        var count = (int)(await command.ExecuteScalarAsync())!;
        return count > 0;
    }
}