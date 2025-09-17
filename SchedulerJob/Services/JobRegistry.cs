using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchedulerJob.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace SchedulerJob.Services
{
    public static class JobRegistry
    {
        public static IDictionary<string, IJobs> CreateJobDictionary(IConfiguration config, ILoggerFactory loggerFactory)
        {
            return new Dictionary<string, IJobs>
            {
                { "enablemanagers", new EnableManagers(config, loggerFactory.CreateLogger<EnableManagers>()) }
                // Add more jobs here as needed
            };
        }
    }
}
