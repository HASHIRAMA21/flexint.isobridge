namespace FlexInt.ISOBridge.Service;

public interface IMessageProcessor
{
    Task ProcessAsync(string xmlFilePath);
}