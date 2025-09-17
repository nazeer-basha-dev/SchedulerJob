using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using SchedulerJob.Services.Interfaces;

namespace SchedulerJob.Test
{
    [TestFixture]
    public class Program_Tests
    {
        //private Mock<ISchedulerServices> _mockSchedulerServices;
        //private IHost _host;

        //[SetUp]
        //public void SetUp()
        //{
        //    _mockSchedulerServices = new Mock<ISchedulerServices>();
        //    _mockSchedulerServices.Setup(s => s.ProcessCronJobsAsync()).Returns(Task.CompletedTask);

        //    _host = Host.CreateDefaultBuilder()
        //        .ConfigureServices(services =>
        //        {
        //            services.AddSingleton(_mockSchedulerServices.Object);
        //        })
        //        .Build();
        //}

        //[Test]
        //public async Task Main_ExecutesSchedulerJobs_CallsProcessCronJobsAsync()
        //{
        //    // Arrange
        //    var args = Array.Empty<string>();
        //    var originalCreateHostBuilder = Program.CreateHostBuilder;
        //    Program.CreateHostBuilder = _ => Host.CreateDefaultBuilder()
        //        .ConfigureServices(services =>
        //        {
        //            services.AddSingleton(_mockSchedulerServices.Object);
        //        });

        //    // Act
        //    await Program.Main(args);

        //    // Assert
        //    _mockSchedulerServices.Verify(s => s.ProcessCronJobsAsync(), Times.Once);

        //    // Cleanup
        //    Program.CreateHostBuilder = originalCreateHostBuilder;
        //}

        //[Test]
        //public void Main_WhenExceptionThrown_LogsError()
        //{
        //    // Arrange
        //    var args = Array.Empty<string>();
        //    _mockSchedulerServices.Setup(s => s.ProcessCronJobsAsync()).ThrowsAsync(new Exception("Test exception"));
        //    var originalCreateHostBuilder = Program.CreateHostBuilder;
        //    Program.CreateHostBuilder = _ => Host.CreateDefaultBuilder()
        //        .ConfigureServices(services =>
        //        {
        //            services.AddSingleton(_mockSchedulerServices.Object);
        //        });

        //    // Act & Assert
        //    Assert.DoesNotThrowAsync(async () => await Program.Main(args));

        //    // Cleanup
        //    Program.CreateHostBuilder = originalCreateHostBuilder;
        //}
    }
}