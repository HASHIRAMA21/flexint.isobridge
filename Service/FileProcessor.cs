using FlexInt.ISOBridge.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FlexInt.ISOBridge.Service;

public class FileProcessor
{
    private readonly DatabaseManager _databaseManager;
    private readonly Iso20022Deserializer _deserializer;
    private readonly ILogger<FileProcessor> _logger;
    private readonly AppSettings _appSettings;

    public FileProcessor(DatabaseManager databaseManager, Iso20022Deserializer deserializer, ILogger<FileProcessor> logger, IConfiguration configuration)
    {
        _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appSettings = configuration.GetSection("AppSettings").Get<AppSettings>() ?? throw new InvalidOperationException("AppSettings is not configured correctly.");
    }
    
    public async Task ProcessFilesAsync()
    {
        var files = Directory.GetFiles(_appSettings.Iso20022FilesDirectory, "*.xml");

        foreach (var file in files)
        {
            try
            {
                if (await _databaseManager.IsFileProcessedAsync(file, _appSettings.ProcessedFilesTableName))
                {
                    _logger.LogInformation($"File already processed: {file}");
                    continue;
                }

                var xmlMessage = await File.ReadAllTextAsync(file);
                var messageType = Path.GetFileNameWithoutExtension(file);

                var data = _deserializer.Deserialize(xmlMessage);

                if (data.Count > 0)
                {
                    // Exclude the 'Id' column from the columns dictionary
                    var columns = data
                        .Where(kvp => kvp.Key != "Id")
                        .ToDictionary(kvp => kvp.Key, kvp => "NVARCHAR(MAX)");

                    await _databaseManager.CreateTableIfNotExistsAsync(messageType, columns);

                    await _databaseManager.InsertDataAsync(messageType, data);
                    await _databaseManager.LogProcessedFileAsync(file, _appSettings.ProcessedFilesTableName);

                    _logger.LogInformation($"Processed file: {file}");
                }
                else
                {
                    _logger.LogWarning($"No data found in file: {file}");
                }
            }
            catch (Exception ex)
            {
                await _databaseManager.LogFailedFileAsync(file, ex.Message, _appSettings.FailedFilesTableName);
                _logger.LogError(ex, $"Error processing file: {file}");
            }
        }
    }

    public async Task ProcessFilesAsyncs()
    {
        var files = Directory.GetFiles(_appSettings.Iso20022FilesDirectory, "*.xml");

        foreach (var file in files)
        {
            try
            {
                if (await _databaseManager.IsFileProcessedAsync(file, _appSettings.ProcessedFilesTableName))
                {
                    _logger.LogInformation($"File already processed: {file}");
                    continue;
                }

                var xmlMessage = await File.ReadAllTextAsync(file);
                var messageType = Path.GetFileNameWithoutExtension(file);

                var data = _deserializer.Deserialize(xmlMessage);

                if (data.Count > 0)
                {
                    var columns = data.ToDictionary(kvp => kvp.Key, kvp => "NVARCHAR(MAX)");
                    await _databaseManager.CreateTableIfNotExistsAsync(messageType, columns);

                    await _databaseManager.InsertDataAsync(messageType, data);
                    await _databaseManager.LogProcessedFileAsync(file, _appSettings.ProcessedFilesTableName);

                    _logger.LogInformation($"Processed file: {file}");
                }
                else
                {
                    _logger.LogWarning($"No data found in file: {file}");
                }
            }
            catch (Exception ex)
            {
                await _databaseManager.LogFailedFileAsync(file, ex.Message, _appSettings.FailedFilesTableName);
                _logger.LogError(ex, $"Error processing file: {file}");
            }
        }
    }
}