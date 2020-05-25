using DddEfteling.Park.Realms.Controls;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Entrances.Controls;
using DddEfteling.Park.Employees.Controls;

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
                services.GetRequiredService<IEntranceControl>();
                services.GetRequiredService<IRideControl>();
                services.GetRequiredService<IEmployeeControl>();
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
