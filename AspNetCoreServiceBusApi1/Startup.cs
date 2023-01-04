using Azure.Storage.Blobs;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ServiceBus.Infraestructure.CommonServices;
using ServiceBusMessaging;
using System.Collections.Generic;

namespace ServiceBusSenderApi
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
            var connectionBus = Configuration.GetConnectionString("ServiceBusConnectionString");
            var conecttionBlob = Configuration.GetConnectionString("AzureStorageAccount");
            services.AddControllers();

            services.AddScoped<ServiceBusSender>();
            services.AddScoped<ServiceBusTopicSender>();
            services.AddScoped<IAzureBlobStorage>(_ => new AzureBlobStorage(new BlobServiceClient(conecttionBlob)));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Payload View API",
                });
            });
            services.AddHealthChecks()
            .AddAzureServiceBusQueue(connectionBus, Configuration["QuebeName"])
            .AddAzureBlobStorage(conecttionBlob.ToString());           
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
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true
            });
            app.UseHealthChecks("/qhealth", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
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
