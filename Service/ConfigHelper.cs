namespace FlexInt.ISOBridge;

using FlexInt.ISOBridge.Data;
using FlexInt.ISOBridge.ParsingRules;
using FlexInt.ISOBridge.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class ConfigHelper
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration();

        // Configuration
        services.AddSingleton<IConfiguration>(configuration);

        // Logging
        services.AddLogging(logging => 
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });

        // Règles de parsing
        var tableRules = ParsingRulesLoader.LoadRulesFromJson(
            configuration["FilePaths:ParsingRulesFile"]);
        services.AddSingleton(tableRules);

        // Services
        services.AddTransient<DatabaseManager>();
        services.AddTransient<Iso20022Deserializer>();
        services.AddTransient<Iso20022Processor>();
        services.AddTransient<FileProcessor>();
        services.AddTransient<ISyncService, SyncService>();

        return services.BuildServiceProvider();
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}