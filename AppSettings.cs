namespace FlexInt.ISOBridge;

public class AppSettings
{
    public string Iso20022FilesDirectory { get; set; } = string.Empty;
    public string ProcessedFilesTableName { get; set; } = string.Empty;
    public string FailedFilesTableName { get; set; } = string.Empty;
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; } = null!;
}
