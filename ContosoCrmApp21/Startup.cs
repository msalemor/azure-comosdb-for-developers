using ContosoCrm.Common21.Models;
using ContosoCrm.DataAccess21.Factories;
using ContosoCrm.DataAccess21.Helpers;
using ContosoCrm.DataAccess21.Interfaces;
using ContosoCrm.DataAccess21.Repositories;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoCrmApp21
{
    public class Startup
    {
        public static string Region;

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

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            DocumentClientFactory.EndpointUri = Configuration["EndpointUri"];
            DocumentClientFactory.AuthKey = Configuration["AuthKey"];
            DocumentClientFactory.PreferredLocations = Configuration["PreferredLocations"];
            Region = Configuration["Region"];

            var telemetryClient = new TelemetryClient();
            services.AddSingleton(telemetryClient);
            services.AddTransient<IDocumentDbHelper<Contact>, ContactDocumentDbRepository>();
            services.AddTransient<IDocumentDbHelper<Company>, CompanyDocumentDbRepository>();
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
