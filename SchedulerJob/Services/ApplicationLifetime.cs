using SchedulerJob.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerJob.Services
{
    public class ApplicationLifetime : IApplicationLifetime
    {
        public async Task Exit(int exitCode)
        {
            await Task.Run(() => Environment.Exit(exitCode));
        }
    }
}
