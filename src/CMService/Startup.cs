using CMService.DAL;
using CMService.Migrations;
using CMService.Settings;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
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

            if (bool.Parse(Configuration["Data:SeedDatabase"])){
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

            services.AddEntityFramework().AddSqlServer().AddDbContext<CustomerDbContext>(
                options => options.UseSqlServer(Configuration.Get("Data:DefaultConnection:ConnectionString")));

            services.Configure<ClientSetting>(Configuration.GetConfigurationSection("Client"));

            services.AddMvc();

            services.AddSwagger();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc(routes =>
            {
                 routes.MapRoute(name: "default", template: "api/{controller}/{action}/");
            });

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
