using System.Data;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace FlexInt.ISOBridge.Service;

public class Iso20022Deserializer
{
    private readonly ILogger<Iso20022Deserializer> _logger;

    public Iso20022Deserializer(ILogger<Iso20022Deserializer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ParseXmlAsync(string filePath, Func<DataSet, Guid, Guid, Task> callback)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            _logger.LogError("File path is null, empty, or does not exist.");
            return;
        }

        try
        {
            string xmlMessage = await File.ReadAllTextAsync(filePath);
            var result = Deserialize(xmlMessage);

            DataSet dataSet = new DataSet();
            // Populate the DataSet from the result dictionary here

            Guid messageUniqueId = Guid.NewGuid();
            Guid uniqueId = Guid.NewGuid();

            await callback(dataSet, messageUniqueId, uniqueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while processing the file: {ex.Message}");
        }
    }

    public Dictionary<string, object> Deserialize(string xmlMessage)
    {
        var result = new Dictionary<string, object>();

        if (string.IsNullOrEmpty(xmlMessage))
        {
            _logger.LogError("xmlMessage is null or empty. Unable to deserialize.");
            return result;
        }

        try
        {
            var doc = XDocument.Parse(xmlMessage);

            foreach (var element in doc.Descendants())
            {
                var key = element.Name.LocalName;
                var value = element.Value;

                if (!result.ContainsKey(key))
                {
                    result[key] = value;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while deserializing the XML: {ex.Message}");
        }

        return result;
    }
}