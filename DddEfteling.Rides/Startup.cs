using DddEfteling.Rides.Boundaries;
using DddEfteling.Rides.Controls;
using DddEfteling.Shared.Boundaries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DddEfteling.Rides
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

            services.AddHttpClient<IEmployeeClient, EmployeeClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["ParkUrl"]);
                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("Application/Json"));
                c.DefaultRequestHeaders.Add("User-Agent", "Efteling");

            })
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(6, _ => TimeSpan.FromSeconds(10)));

            services.AddHttpClient<IVisitorClient, VisitorClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["VisitorUrl"]);
                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("Application/Json"));
                c.DefaultRequestHeaders.Add("User-Agent", "Efteling");

            })
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(6, _ => TimeSpan.FromSeconds(10)));


            services.AddControllers();
            services.AddSingleton<IEventConsumer, EventConsumer>();
            services.AddSingleton<IEventProducer, EventProducer>();
            services.AddSingleton<IRideControl, RideControl>();
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: DefaultCorsPolicy,
                builder =>
                {
                    builder.WithOrigins("http://localhost:3999", "http://localhost:3998", "http://localhost:3997", "http://localhost:3996", "http://localhost:3995");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEventConsumer eventConsumer,
            IRideControl rideControl)
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

            _ = Task.Run(() => eventConsumer.Listen());

            _ = Task.Run(() =>
            {
                while (true)
                {
                    rideControl.HandleOpenRides();
                    Task.Delay(1000).Wait();
                }
            });
        }
    }
}
