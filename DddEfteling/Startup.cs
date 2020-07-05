using DddEfteling.Park.Common.Control;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Entrances.Controls;
using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Stands.Controls;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using DddEfteling.Park.Visitors.Entities;
using DddEfteling.Park.Visitors.Controls;

namespace DddEfteling
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        IConfiguration Configuration { get; }

        /* This method gets called by the runtime. Use this method to add services to the container.
         For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        */
        public void ConfigureServices(IServiceCollection services)
        {
            var visitorSettingsSection = Configuration.GetSection("VisitorSettings");
            services.Configure<VisitorSettings>(visitorSettingsSection);

            services.AddMediatR(typeof(Startup));
            services.AddSingleton<IRealmControl, RealmControl>();
            services.AddSingleton<INameService, NameService>();
            services.AddSingleton<IEntranceControl, EntranceControl>();
            services.AddSingleton<IRideControl, RideControl>();
            services.AddSingleton<IFairyTaleControl, FairyTaleControl>();
            services.AddSingleton<IStandControl, StandControl>();
            services.AddSingleton<IEmployeeControl, EmployeeControl>();
            services.AddSingleton<IVisitorControl, VisitorControl>();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
