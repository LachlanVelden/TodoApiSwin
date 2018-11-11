using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Define the version of MVC that we wish to use (this should match the current .net core version)
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add The Swagger Documentation Generator Service
            services.AddSwaggerGen(c =>
            {
                // Define an API with v1
                c.SwaggerDoc("v1", new Info { Title = "Todo API", Version = "v1" });
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
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
        }
    }
}
