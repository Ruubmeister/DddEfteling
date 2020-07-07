using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Entrances.Controls;
using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Stands.Controls;
using DddEfteling.Park.Visitors.Controls;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace DddEfteling
{
    public class Program
    {
        protected Program()
        {

        }

        public static async Task Main(string[] args)
        {
            

            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                services.GetRequiredService<IRealmControl>();
                IEntranceControl entranceControl = services.GetRequiredService<IEntranceControl>();
                services.GetRequiredService<IRideControl>();
                services.GetRequiredService<IFairyTaleControl>();
                services.GetRequiredService<IStandControl>();
                services.GetRequiredService<IEmployeeControl>();
                IVisitorControl visitorControl = services.GetRequiredService<IVisitorControl>();

                Task.Run(async () =>
               {
                   Random random = new Random();
                   int maxVisitors = 1000;
                   int currentVisitors = 0;
                   while (currentVisitors <= maxVisitors)
                   {
                       int newVisitors = random.Next(10, 20);

                       visitorControl.AddVisitors(newVisitors);
                       currentVisitors += newVisitors;
                       Task.Delay(1000).Wait();
                   }
               });
                

            }

            await host.RunAsync();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
