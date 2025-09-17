using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SchedulerJob.Constants;
using SchedulerJob.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerJob.Helper
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationHelper
    {
        private static IHostEnvironment _hostingEnvironment = default!;

        public static void Initialize(IHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        //public static string GetConfigurationValue(this IConfiguration configuration, string key)
        //{
        //    return IsLocalEnvironment()
        //        ? configuration[key] ?? string.Empty
        //        : configuration.GetVaultKubernetesAppSecret(key);
        //}

        public static ConnectionStrings GetConnecitonString(this IConfiguration config)
        {
            try
            {
                IConfigurationSection connConfigSection = config.GetSection(VaultKeyConstants.ConnectionStrings);
                return connConfigSection.Get<ConnectionStrings>() ?? new ConnectionStrings();
                //if (IsLocalEnvironment())
                //{
                //    IConfigurationSection connConfigSection = config.GetSection(VaultKeyConstants.ConnectionStrings);
                //    return connConfigSection.Get<ConnectionStrings>() ?? new ConnectionStrings();
                //}
                //else
                //{

                //    var settings = config.GetConfigurationValue(VaultKeyConstants.ConnectionStrings);
                //    return JsonConvert.DeserializeObject<ConnectionStrings>(settings) ?? new ConnectionStrings();
                //}
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
        }

        public static bool IsLocalEnvironment()
        {
            return _hostingEnvironment.EnvironmentName.Equals(ApplicationConstants.LocalEnvironment, StringComparison.InvariantCultureIgnoreCase);
        }

        public static T GetSection<T>(this IConfiguration configuration, string key) where T : new()
        {
            var settings = new T();
            if (IsLocalEnvironment())
            {
                configuration.GetSection(key).Bind(settings);
            }
            //else
            //{
            //    var configValue = configuration.GetConfigurationValue(key);
            //    var deserialized = JsonConvert.DeserializeObject<T>(configValue);
            //    if (deserialized != null)
            //    {
            //        settings = deserialized;
            //    }
            //}
            return settings;
        }

        public static string GetSectionKeyValue(this IConfiguration configuration, string key)
        {
            string? response;
            response = configuration.GetSection(key).Value;
            //if (IsLocalEnvironment())
            //{
            //    response = configuration.GetSection(key).Value;
            //}
            //else
            //{
            //    response = configuration.GetConfigurationValue(key);
            //}
            return response ?? string.Empty;
        }
    }
}
