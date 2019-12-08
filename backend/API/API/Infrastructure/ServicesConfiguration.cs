using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Swagger;

using API.Services;
using API.HostedServices;
using API.Infrastructure.Swagger;

using QueueService.Models;
using QueueService.Interfaces;
using QueueService.QueueServices;

using RabbitMQ.Client;

namespace API.Infrastructure
{
    public static class ServicesConfiguration
    {
        public static IConfiguration BuildConfiguration(IHostingEnvironment hostingEnvironment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        public static void AddBusinessLogicServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<FileService>();
            services.AddScoped<MessageService>();
        }

        public static void AddBackgroundsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<IsSolvedService>();
        }

        public static void AddQueueSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Settings>("PostFileQueue", configuration.GetSection("RabbitMq:FileData"));
            services.Configure<Settings>("DownloadFileQueue", configuration.GetSection("RabbitMq:Result"));
            services.Configure<Settings>("IsSolvedQueue", configuration.GetSection("RabbitMq:IsSolved"));
        }

        public static void AddMessageServices(this IServiceCollection services, IConfiguration configuration)
        {
            string hostName = configuration.GetValue<string>("RabbitMq:HostName");
            services.AddSingleton<IConnectionFactory, DefaultConnectionFactory>(f => new DefaultConnectionFactory(hostName));
            services.AddSingleton<IConnectionProvider, ConnectionProvider>();
        }
        
        public static void UseCORS(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseCors(builder =>
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins(configuration.GetSection("CORS:AllowedOrigin").Get<string>()));
        }

        #region Swagger
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(conf =>
            {
                conf.SwaggerDoc(name: "v1", info: new Info { Title = "API", Version = "v1" });
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
