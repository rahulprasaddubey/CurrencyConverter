using Quartz;
using Quartz.Impl;

namespace CurrencyConverter
{
    public class RateDaemon
    {
         public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<RateExchangeJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger0", "group0")
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(5)
                .RepeatForever()).Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}