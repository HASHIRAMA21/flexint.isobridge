
```markdown
# ISO 20022 File Processing Library
```

## Installation

To use this library, you need to add it to your .NET project. You can do this using NuGet. Run the following command in the Package Manager Console:

```bash
 Install-Package FlexInt.ISOBridge
```

Replace `YourLibraryName` with the actual name of your package.

## Configuration

### `appsettings.json` File

You need to add an `appsettings.json` file to the root of your project with the necessary configurations. Here’s an example file:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "FilePaths": {
    "Iso20022Directory": "path/to/iso20022/files",
    "ParsingRulesFile": "path/to/parsing_rules.json"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
  }
}
```

### Sections of the File

- **Logging**: Configures the logging level.
- **FilePaths**: Specifies the paths for ISO 20022 files and parsing rules.
- **ConnectionStrings**: Contains the connection string for the SQL Server database.

## Usage

### Code Example

Here’s an example to get started with the library:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder => builder.AddConsole());
                services.AddSingleton<DatabaseManager>();
                services.AddSingleton<Iso20022Deserializer>();
                services.AddSingleton<FileProcessor>();
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .Build();

        var logger = host.Services.GetService<ILogger<Program>>();
        var databaseManager = host.Services.GetService<DatabaseManager>();
        var deserializer = host.Services.GetService<Iso20022Deserializer>();
        var fileProcessor = host.Services.GetService<FileProcessor>();

        try
        {
            var configuration = host.Services.GetService<IConfiguration>();
            string directoryPath = configuration["FilePaths:Iso20022Directory"];
            string parsingRulesPath = configuration["FilePaths:ParsingRulesFile"];
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            // Load parsing rules
            var tableRules = ParsingRulesLoader.LoadRulesFromJson(parsingRulesPath);

            // Start processing files
            await fileProcessor.ProcessFilesAsync(directoryPath);

            logger.LogInformation("Processing completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing files.");
        }
    }
}
```

## Available Services

- **DatabaseManager**: Manages interactions with the SQL Server database.
- **Iso20022Deserializer**: Deserializes ISO 20022 files.
- **FileProcessor**: Processes ISO 20022 files based on parsing rules.

## Contributing

Contributions are welcome! If you want to contribute to this project, please follow these steps:

1. Fork the project.
2. Create a branch for your feature (`git checkout -b feature/NewFeature`).
3. Commit your changes (`git commit -m 'Add a new feature'`).
4. Push the branch (`git push origin feature/NewFeature`).
5. Open a pull request.

## License

This project is licensed under the SYNTHI AI License. See the [LICENSE](LICENSE) file for more information.
```