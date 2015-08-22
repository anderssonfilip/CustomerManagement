using CMService.Migrations;
using CMService.Settings;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using System;

namespace CMService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            var configBuilder = new ConfigurationBuilder(appEnv.ApplicationBasePath);
            configBuilder.AddJsonFile("config.json");
            configBuilder.AddEnvironmentVariables();
            Configuration = configBuilder.Build();

            if(Boolean.Parse(Configuration["Data:SeedDatabase"])){
                SeedCustomers.Seed(Configuration["Data:DefaultConnection:ConnectionString"]);
            }
        }

        public IConfiguration Configuration
        {
            get;
            private set;
        }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DbSetting>(Configuration.GetConfigurationSection("Data:DefaultConnection"));
            services.Configure<ClientSetting>(Configuration.GetConfigurationSection("Client"));

            services.AddMvc();

        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc(routes =>
            {
                 routes.MapRoute(name: "default", template: "api/{controller}/{action}/");
            });
        }
    }
}
