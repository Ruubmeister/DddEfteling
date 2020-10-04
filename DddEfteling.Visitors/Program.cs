using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DddEfteling.Shared.Boundary;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Boundary;
using DddEfteling.Visitors.Controls;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DddEfteling.Visitors
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                services.GetRequiredService<IRideClient>();
                services.GetRequiredService<IFairyTaleClient>();
                services.GetRequiredService<IEventProducer>();
                services.GetRequiredService<IRideClient>();
                

                IVisitorControl visitorControl = services.GetRequiredService<IVisitorControl>();
                IEventConsumer eventConsumer = services.GetRequiredService<IEventConsumer>();

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
                    int maxVisitors = 5000;
                    int currentVisitors = 0;
                    while (currentVisitors <= maxVisitors)
                    {
                        int newVisitors = random.Next(2, 10);

                        visitorControl.AddVisitors(newVisitors);
                        currentVisitors += newVisitors;
                        Task.Delay(5000).Wait();
                    }
                });

                await host.RunAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
