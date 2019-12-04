using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Swagger;

using API.Services;
using API.HostedServices;
using API.Infrastructure.Swagger;

using QueueService.Interfaces;
using QueueService.QueueServices;

using RabbitMQ.Client;

namespace API.Infrastructure
{
    public static class ServicesConfiguration
    {
        public static void AddBusinessLogicServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<FileService>();
            services.AddScoped<MessageService>();
        }

        public static void AddBackgroundsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<IsSolvedService>();
        }

        public static void AddMessageServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionFactory, DefaultConnectionFactory>();
            services.AddSingleton<IConnectionProvider, ConnectionProvider>();
        }


        #region Swagger
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(conf =>
            {
                conf.SwaggerDoc(name: "v1", info: new Info() { Title = "API", Version = "v1" });
                conf.OperationFilter<FileUploadOperation>();
            });
        }

        public static void UseSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(opt =>
            {
                opt.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint(url: "v1/swagger.json", name: "API");
            });
        }
        #endregion
    }
}
