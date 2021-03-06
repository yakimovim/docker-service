using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Controllers;
using Service.Hubs;
using Service.Model;
using Service.Services;

namespace Service
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
            services.AddMvc();

            services.AddSignalR();

            services.AddServerSideBlazor();

            services.AddScoped<HomePageModelFactory>();

            services.AddSingleton<Health>();
            services.Configure<HealthConfig>(Configuration.GetSection("DockerService:Health"));

            services.AddSingleton<MemoryLoadProvider>();

            services.AddHttpClient();

            services.AddScoped<RequestSender>();

            services.AddSingleton<ResponseDefinitions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapDefaultControllerRoute();

                endpoints.MapHub<HealthHub>("/hubs/health");

                endpoints.MapBlazorHub();
            });
        }
    }
}
