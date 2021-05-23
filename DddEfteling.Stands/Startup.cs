using System;
using System.Threading.Tasks;
using DddEfteling.Shared.Controls;
using DddEfteling.Stands.Boundaries;
using DddEfteling.Stands.Controls;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DddEfteling.Stands
{
    public class Startup
    {
        private readonly string DefaultCorsPolicy = "_defaultCorsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Random>();
            services.AddSingleton<IStandControl, StandControl>();
            services.AddSingleton<IEventProducer, EventProducer>();
            services.AddSingleton<ILocationService, LocationService>();
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: DefaultCorsPolicy,
                builder =>
                {
                    builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStandControl standControl)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(DefaultCorsPolicy);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            _ = Task.Run(() =>
            {
                while (true)
                {
                    standControl.HandleProducedOrders();
                    Task.Delay(1000).Wait();
                }
            });
        }
    }
}
