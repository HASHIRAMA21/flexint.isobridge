using FlexInt.ISOBridge.Data;
using FlexInt.ISOBridge.ParsingRules;
using FlexInt.ISOBridge.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlexInt.ISOBridge;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var parsingRulesFilePath = configuration["FilePaths:ParsingRulesFile"];
        if (string.IsNullOrEmpty(parsingRulesFilePath))
        {
            throw new InvalidOperationException("ParsingRulesFile path is not configured.");
        }

        var tableRules = ParsingRulesLoader.LoadRulesFromJson(parsingRulesFilePath);
        if (tableRules == null)
        {
            throw new InvalidOperationException("Parsing rules could not be loaded.");
        }

        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug))
            .AddSingleton(tableRules)
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<DatabaseManager>(sp =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("DefaultConnection is not configured.");
                }

                var logger = sp.GetRequiredService<ILogger<DatabaseManager>>();
                return new DatabaseManager(connectionString, logger);
            })
            .AddSingleton<Iso20022Deserializer>()
            .AddSingleton<FileProcessor>()
            .BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var fileProcessor = serviceProvider.GetRequiredService<FileProcessor>();

        logger.LogInformation("=== Démarrage du programme ===");

        try
        {
            logger.LogInformation("Début du traitement des fichiers...");
            await fileProcessor.ProcessFilesAsync();
            logger.LogInformation("Traitement terminé avec succès.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Une erreur s'est produite lors du traitement des fichiers.");
            throw;
        }
    }
}