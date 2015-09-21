using CMService.DAL;
using CMService.Migrations;
using CMService.Models;
using CMService.Settings;
using Entities;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Data.Entity;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

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

            if (bool.Parse(Configuration["Data:SeedDataOnStartup"]))
            {
                if (Configuration.GetSection("Persistence").Value.Equals("SQL"))
                {
                    SeedCustomers.SeedToMSSQL(Configuration["Data:SQL:ConnectionString"]);
                }
                else if (Configuration.GetSection("Persistence").Value.Equals("Graph"))
                {
                    SeedCustomers.SeedToNeo4j(Configuration["Data:Graph:ConnectionString"]);
                }
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
            var persistence = Configuration.GetSection("Persistence").Value;

            if (persistence.Equals("SQL"))
            {
                services.AddEntityFramework().AddSqlServer().AddDbContext<CustomerDbContext>(
                    options => options.UseSqlServer(Configuration.GetSection("Data:SQL:ConnectionString").Value));

                services.AddScoped<IRepository<Customer>, CustomerRepository>();
            }
            else if (persistence.Equals("Graph"))
            {
                services.Configure<GraphSetting>(Configuration.GetSection("Data:Graph"));
                services.AddScoped<IRepository<Customer>, CustomerGraph>();
            }

            services.Configure<ClientSetting>(Configuration.GetSection("Client"));

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

            //if (bool.Parse(Configuration["Data:SeedDataOnStartup"]))
            //{
            //    app.UseStaticFiles();  // Serve the SampleData.csv file
            //}

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
