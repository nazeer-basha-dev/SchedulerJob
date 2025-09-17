using Microsoft.Extensions.Logging;
using SchedulerJob.Constants;
using SchedulerJob.Jobs;
using SchedulerJob.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace SchedulerJob.Services
{
    public class SchedulerServices : ISchedulerServices
    {
        private readonly ILogger<SchedulerServices> _logger;
        private readonly IApplicationLifetime _appLifetime;
        private readonly IDictionary<string, IJobs> _jobs;

        public SchedulerServices(
            ILogger<SchedulerServices> logger,
            IApplicationLifetime appLifetime,
            IDictionary<string, IJobs> jobs)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _jobs = jobs;
        }

        public async Task ProcessCronJobsAsync()
        {
            var jobType = Environment.GetEnvironmentVariable(ApplicationConstants.JobType)?.ToLowerInvariant();
            _logger.LogInformation("JobType : {JobType}", jobType);

            if (string.IsNullOrWhiteSpace(jobType) || !_jobs.TryGetValue(jobType, out var job))
            {
                _logger.LogWarning("Unknown job type: {JobType}", jobType);
                await _appLifetime.Exit(0);
                return;
            }

            try
            {
                await job.ExecuteAsync();
                await _appLifetime.Exit(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing job type: {JobType}", jobType);
                await _appLifetime.Exit(1);
            }
        }
    }
}
