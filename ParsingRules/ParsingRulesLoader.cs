namespace FlexInt.ISOBridge.ParsingRules;

using System.IO;
using Newtonsoft.Json;

public class ParsingRulesLoader
{
    public static TableRules LoadRulesFromJson(string jsonFilePath)
    {
        if (string.IsNullOrEmpty(jsonFilePath))
            throw new ArgumentNullException(nameof(jsonFilePath));

        if (!File.Exists(jsonFilePath))
            throw new FileNotFoundException("JSON file not found", jsonFilePath);

        string json = File.ReadAllText(jsonFilePath);
        return JsonConvert.DeserializeObject<TableRules>(json) ?? throw new InvalidOperationException("Failed to deserialize JSON.");
    }
}