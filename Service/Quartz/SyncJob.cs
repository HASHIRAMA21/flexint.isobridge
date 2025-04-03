namespace FlexInt.ISOBridge;
using FlexInt.ISOBridge.Service;
using Quartz;
using System.Threading.Tasks;

public class SyncJob : IJob
{
    private readonly FileProcessor _fileProcessor;

    public SyncJob(FileProcessor fileProcessor)
    {
        _fileProcessor = fileProcessor;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _fileProcessor.ProcessFilesAsync();
    }
}
