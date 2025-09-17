using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerJob.Jobs
{
    public class EnableManagers : IJobs
    {
        private readonly ILogger<EnableManagers> _logger;
        private readonly IConfiguration _configuration;

        public EnableManagers(IConfiguration configuration, ILogger<EnableManagers> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Enabling managers process started.");

            try
            {
                await EnableManagersAsync();
                _logger.LogInformation("Enabling managers process completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while enabling managers.");
                throw; // Rethrow for testability
            }
        }

        // Separated logic for easier testing/mocking
        protected virtual Task EnableManagersAsync()
        {
            // TODO: Implement actual enable logic here
            return Task.CompletedTask;
        }
    }
}
