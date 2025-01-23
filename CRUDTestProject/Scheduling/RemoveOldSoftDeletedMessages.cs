using Cronos;
using CRUDTestProject.Data;
using EasyCronJob.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRUDTestProject.Scheduling
{
    public class RemoveOldSoftDeletedMessages(
            ICronConfiguration<RemoveOldSoftDeletedMessages> cronConfiguration,
            IServiceProvider serviceProvider,
            ILogger<RemoveOldSoftDeletedMessages> logger) : CronJobService(cronConfiguration.CronExpression, cronConfiguration.TimeZoneInfo)
    {
        const int DAYS = 31;
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Time: {DateTime.Now} the recurring job of removing old soft deleted messages is active");

            return base.StartAsync(cancellationToken);
        }

        protected override Task ScheduleJob(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Time: {DateTime.Now} scheduling next removal of soft deleted messages");
            return base.ScheduleJob(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Removing messages that have been soft deleted for more than {DAYS} days. Start Time : {DateTime.Now}");
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {

                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    DateTime removeBefore = DateTime.Now - TimeSpan.FromDays(DAYS);

                    var affected = dbContext.Messages.Where(m => m.IsDeleted)
                                    .Where(m => m.DeletedOn < removeBefore).ExecuteDelete();

                    logger.LogInformation("Removed: " + affected + " old soft deleted messages");

                }
            }
            catch (Exception e) 
            {
                logger.LogError(e, "Unexpected error in RemoveOldSoftDeletedMessages");
            }
            return base.DoWork(cancellationToken);
        }
    }
}
