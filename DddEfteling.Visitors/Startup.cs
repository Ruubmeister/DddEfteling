using DddEfteling.Shared.Boundaries;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Controls;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Entities;

namespace DddEfteling.Visitors
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
            services.AddHttpClient<IRideClient, RideClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["RideUrl"]);
                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("Application/Json"));
                c.DefaultRequestHeaders.Add("User-Agent", "Efteling");

            })
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(6, _ => TimeSpan.FromSeconds(10)));

            services.AddHttpClient<IFairyTaleClient, FairyTaleClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["FairyTaleUrl"]);
                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("Application/Json"));
                c.DefaultRequestHeaders.Add("User-Agent", "Efteling");

            })
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(6, _ => TimeSpan.FromSeconds(10)));

            services.AddHttpClient<IStandClient, StandClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration["StandUrl"]);
                c.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("Application/Json"));
                c.DefaultRequestHeaders.Add("User-Agent", "Efteling");

            })
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(6, _ => TimeSpan.FromSeconds(10)));

            services.AddSingleton<IVisitorRepository, VisitorRepository>();
            services.AddSingleton<ILocationTypeStrategy, LocationTypeStrategy>();
            services.AddSingleton<IVisitorMovementService, VisitorMovementService>();
            services.AddSingleton<IEventProducer, EventProducer>();
            services.AddSingleton<IEventConsumer, EventConsumer>();
            services.AddSingleton<IVisitorControl, VisitorControl>();
            services.AddMediatR(typeof(Startup));
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IEventConsumer eventConsumer, IVisitorControl visitorControl, ILocationTypeStrategy locationTypeStrategy,
            IEventProducer producer, IFairyTaleClient fairyTaleClient, IRideClient rideClient, IStandClient standClient)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            locationTypeStrategy.Register(LocationType.FAIRYTALE, 
                new VisitorFairyTaleStrategy(producer, fairyTaleClient));
            locationTypeStrategy.Register(LocationType.RIDE, 
                new VisitorRideStrategy(producer, rideClient));
            locationTypeStrategy.Register(LocationType.STAND, 
                new VisitorStandStrategy(producer, standClient));

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
                    visitorControl.HandleIdleVisitors();

                    Task.Delay(300).Wait();
                }
            });

            _ = Task.Run(() =>
            {
                Random random = new Random();
                int maxVisitors = 15000;
                int currentVisitors = 0;
                while (currentVisitors <= maxVisitors)
                {
                    int newVisitors = random.Next(5, 15);

                    visitorControl.AddVisitors(newVisitors);
                    currentVisitors += newVisitors;
                    Task.Delay(2000).Wait();
                }
            });
        }
    }
}
