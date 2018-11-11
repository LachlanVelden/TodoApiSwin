using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using WebApplication1.Models;

namespace WebApplication1
{
    /// <summary>
    /// An object defining 2 methods to be called at the initial start of the server
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Create an instance of this startup class, and get any required services from dependency injection
        /// </summary>
        /// <param name="configuration">The servers configuration model collected from dependency injection</param>
        /// <param name="environment">The hosting environment's configuration model collected from dependency injection</param>
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        /// This servers configuration model
        /// </summary>
        /// <remarks>This will include appsettings.{configuration}.json and environment variables</remarks>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// This hosting environment's configuration model
        /// </summary>
        public IHostingEnvironment Environment { get; set; }

        /// <summary>
        /// Configure the server's services.
        /// </summary>
        /// <remarks>This method gets called by the runtime.</remarks>
        /// <param name="services">The default service collection of this web service </param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Define the version of MVC that we wish to use (this should match the current .net core version)
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add the Entity Framework services
            services.AddDbContext<ApplicationDatabaseContext>(options =>
            {
                // On an Azure App service:  This will use values from App -> Application Settings -> Connection strings
                // In Development: This will use a local database file (as seen in appsettings.Development.json)
                options.UseSqlServer(Configuration["ConnectionStrings:defaultConnection"]);
            });

            // Add The Swagger Documentation Generator services
            services.AddSwaggerGen(c =>
            {
                // Define an API with v1
                c.SwaggerDoc("v1", new Info { Title = "Todo API", Version = "v1" });
                // Tell Swagger to use the compiler generated XML documentation
                // {0} - The name of the current namespace / the exported XML file name
                var xmlDocsPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    string.Format(@"{0}.xml",
                    Assembly.GetExecutingAssembly().GetName().Name)
                );
                c.IncludeXmlComments(xmlDocsPath);
            });

            // Add the CORS services so that we can access this API from external hosts
            // * Google CORS for more info *
            services.AddCors(builder =>
            {
                // Add a default policy
                builder.AddPolicy("default", options =>
                {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.AllowAnyOrigin();
                });
            });
        }


        /// <summary>
        /// Configure the HTTP Request pipeline.
        /// </summary>
        /// <remarks>This method gets called by the runtime after all services have been added.</remarks>
        /// <param name="app">The IApplicationBuilder to configure the application's request pipeline </param>
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                // The server is in development mode, we want to present the error messages towards the developer. Hiding them would be stupid
                app.UseDeveloperExceptionPage();
            }

            // Use the 'default' CORS policy 
            app.UseCors("default");

            // Use the MVC stack - This will enable routing and view rendering
            app.UseMvc();
            // Use the Swagger documentation generator
            app.UseSwagger();
            // Use the Swagger UI - This will add the Swagger UI resources to /swagger
            app.UseSwaggerUI(c =>
            {

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API");
            });

            // Automatically migrate the database (and create it if it does not already exist)
            InitializeDatabase(app);
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>().Database.Migrate();
            }
        }
    }
}
