using ContosoCrm.Common21.Models;
using ContosoCrm.DataAccess21.Helpers;
using ContosoCrm.DataAccess21.Interfaces;
using ContosoCrm.DataAccess21.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ContosoCrmSPA.Service
{
    public class Startup
    {
        public static string Region;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            loggerFactory.AddConsole();
        }

        public IConfiguration Configuration { get; }
        public ILoggerFactory LoggerFactor { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            DocumentDbClientInstance.EndpointUri = Configuration["EndpointUri"];
            DocumentDbClientInstance.AuthKey = Configuration["AuthKey"];
            DocumentDbClientInstance.PreferredLocations = Configuration["PreferredLocations"];
            Region = Configuration["Region"];

            services.AddTransient<IDocumentDbHelper<Contact>, ContactDocumentDbRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
