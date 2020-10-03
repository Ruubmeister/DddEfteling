using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using DddEfteling.Shared.Controls;
using DddEfteling.Park.Boundaries;
using DddEfteling.Park.Controls;

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

            services.AddMediatR(typeof(Startup));
            services.AddSingleton<IEventProducer, EventProducer>();
            services.AddSingleton<INameService, NameService>();
            services.AddSingleton<IRealmControl, RealmControl>();
            services.AddSingleton<IEntranceControl, EntranceControl>();
            services.AddSingleton<IEmployeeControl, EmployeeControl>();
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
