using Azure.Storage.Blobs;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ServiceBus.Infraestructure.CommonServices;
using ServiceBusMessaging;
using ServiceBusReceiverApi.Handlers;
using ServiceBusReceiverApi.Model;
using System.Collections.Generic;
using System.Linq;

namespace ServiceBusReceiverApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var connectionSql = Configuration.GetConnectionString("DefaultConnection");
            var connectionBus = Configuration.GetConnectionString("ServiceBusConnectionString");
            var conecttionBlob = Configuration.GetConnectionString("AzureStorageAccount");
            services.AddDbContext<PayloadContext>(options =>
                options.UseMySql(connectionSql, ServerVersion.AutoDetect(connectionSql))
                );
            services.AddSingleton<IServiceBusConsumer, ServiceBusConsumer>();
            services.AddSingleton<ServiceBusSender>();
            services.AddSingleton<IServiceBusTopicSubscription, ServiceBusTopicSubscription>();
            services.AddSingleton<IProcessData, ProcessData>();
            services.AddScoped<IAzureBlobStorage>(_ => new AzureBlobStorage(new BlobServiceClient(conecttionBlob)));
            //**activar servicio=> services.AddHostedService<WorkerServiceBusReceiver>();**//

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Payload API",
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
            services.AddHealthChecks().AddMySql(connectionSql).AddAzureBlobStorage(conecttionBlob).AddAzureServiceBusQueue(connectionBus, Configuration["QuebeNameOut"]);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true
            });
            app.UseHealthChecks("/qhealth", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            // Run handlers
            app.UseEventHandler();
            app.UseSwagger(options =>
            {
                options.SerializeAsV2 = false;
                options.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                });
            });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payload Management API V1"));
        }
    }
}
