using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerJob.Services.Interfaces
{
    public interface ISchedulerServices
    {
        Task ProcessCronJobsAsync();
    }
}
