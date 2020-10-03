using DddEfteling.Park.Boundaries;
using DddEfteling.Park.Controls;
using DddEfteling.Shared.Controls;
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
                services.GetRequiredService<IEventProducer>();
                services.GetRequiredService<INameService>();
                IEntranceControl entranceControl = services.GetRequiredService<IEntranceControl>();
                services.GetRequiredService<IEmployeeControl>();

                entranceControl.OpenPark();
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
