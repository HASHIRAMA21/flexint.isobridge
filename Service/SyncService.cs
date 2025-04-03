namespace FlexInt.ISOBridge;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

public interface ISyncService
{
    Task StartAsync();
    Task StopAsync();
}

public class SyncService : ISyncService
{
    private readonly IScheduler _scheduler;
    private readonly IServiceProvider _serviceProvider;

    public SyncService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
        _scheduler.JobFactory = new QuartzJobFactory(_serviceProvider);
    }

    public async Task StartAsync()
    {
        var job = JobBuilder.Create<SyncJob>()
            .WithIdentity("syncJob", "group1")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("syncTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(_serviceProvider.GetService<IConfiguration>()
                    .GetValue<int?>("SyncSettings:IntervalMinutes") ?? 45)
                .RepeatForever())
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
        await _scheduler.Start();
    }

    public async Task StopAsync()
    {
        await _scheduler.Shutdown(true);
    }
}
