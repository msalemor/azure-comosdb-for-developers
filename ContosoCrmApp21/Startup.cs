using ContosoCrm.Common21.Models;
using ContosoCrm.DataAccess21.Factories;
using ContosoCrm.DataAccess21.Interfaces;
using ContosoCrm.DataAccess21.Repositories;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Timers;

namespace ContosoCrmApp21
{
    public class Startup
    {
        public static string Region;
        private TelemetryClient telemetryClient = new TelemetryClient();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
            {
                cacheOptions.ContainerName = Configuration["CosmosCacheContainer"];
                cacheOptions.DatabaseName = Configuration["CosmosCacheDatabase"];
                cacheOptions.ClientBuilder = new CosmosClientBuilder(Configuration["CosmosConnectionString"]);

                cacheOptions.CreateIfNotExists = true;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(3600);
                options.Cookie.IsEssential = true;
            });

            DocumentClientFactory.EndpointUri = Configuration["EndpointUri"];
            DocumentClientFactory.AuthKey = Configuration["AuthKey"];
            DocumentClientFactory.PreferredLocations = Configuration["PreferredLocations"];
            Region = Configuration["Region"];

            //var configuration = TelemetryConfiguration.CreateDefault();
            telemetryClient.Context.Cloud.RoleName = "ContosoCrm";

            services.AddSingleton(telemetryClient);

            //FlushTelemetry();
            services.AddTransient<IDocumentDbHelper<Contact>, ContactDocumentDbRepository>();
            services.AddTransient<IDocumentDbHelper<Company>, CompanyDocumentDbRepository>();
            services.AddTransient<IDocumentDbHelper<Lookup>, LookupDocumentDbRepository>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        //private Timer timer;

        //public void FlushTelemetry()
        //{
        //    timer = new Timer
        //    {
        //        Interval = 2000,
        //        Enabled = true
        //    };
        //    timer.Elapsed += Timer_Elapsed;
        //}

        //private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    timer.Enabled = false;
        //    // Allow time for flushing:
        //    System.Threading.Thread.Sleep(1000);
        //    timer.Enabled = true;
        //}

    }
}
