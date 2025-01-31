using CRUDTestProject.Services;
using EasyCronJob.Abstractions;

namespace CRUDTestProject.Scheduling
{
    public class BackupMessagesDb(
            ICronConfiguration<BackupMessagesDb> cronConfiguration,
            IServiceProvider serviceProvider,
            ILogger<BackupMessagesDb> logger) : CronJobService(cronConfiguration.CronExpression, cronConfiguration.TimeZoneInfo)
    {
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Time: {DateTime.Now} the recurring job of backing up the messages database is active");

            return base.StartAsync(cancellationToken);
        }

        protected override Task ScheduleJob(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Time: {DateTime.Now} scheduling next backup of the messages database");
            return base.ScheduleJob(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Backing up the messages database. Start Time : {DateTime.Now}");

            try
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IBackupHandler>();

                handler.Create().Wait();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected error in BackupMessagesDb");
            }
            return base.DoWork(cancellationToken);
        }
    }
}
