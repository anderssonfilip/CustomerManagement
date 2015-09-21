using CMClient.Settings;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

namespace CMClient
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            var configBuilder = new ConfigurationBuilder(appEnv.ApplicationBasePath);
            configBuilder.AddJsonFile("config.json");
            configBuilder.AddEnvironmentVariables();
            Configuration = configBuilder.Build();
        }

        public IConfiguration Configuration
        {
            get;
            private set;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServiceSetting>(Configuration.GetSection("Service"));

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(r => {
                r.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Customer", action = "Search"  });
            });

            app.UseStaticFiles();
        }
    }
}
