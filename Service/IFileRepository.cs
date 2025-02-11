namespace FlexInt.ISOBridge.Service;

public interface IFileRepository
{
    bool IsFileProcessed(string filePath);
    void MarkFileAsProcessed(string filePath);
    void LogError(string filePath, string errorMessage);
}