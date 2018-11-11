using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    /// <summary>
    /// The Standard Program Class that is run when this application is started
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The Entry point of this application
        /// </summary>
        /// <param name="args">Provided console arguments at launch</param>
        public static void Main(string[] args)
        {
            // Create the builder for this server
            var hostBuilder = CreateWebHostBuilder(args);
            // Build the configuration and run the server
            hostBuilder.Build().Run();
        }

        /// <summary>
        /// Create a new WebHostBuilder Model
        /// </summary>
        /// <param name="args">Provided console arguments at launch</param>
        /// <returns>The builder for this server</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => { config.AddEnvironmentVariables(); })
                .UseStartup<Startup>();
        }
            
    }
}
