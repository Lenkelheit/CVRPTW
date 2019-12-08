using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using API.Infrastructure;

namespace API
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            Configuration = ServicesConfiguration.BuildConfiguration(hostingEnvironment);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger(Configuration);
            services.AddBusinessLogicServices(Configuration);
            services.AddBackgroundsServices(Configuration);
            services.AddQueueSettings(Configuration);
            services.AddMessageServices(Configuration);
            services.AddSignalR();
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

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

            app.UseSwagger();
            app.UseCORS(Configuration);
            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<Hubs.OrToolsHub>("/or-tools");
            });
        }
    }
}
