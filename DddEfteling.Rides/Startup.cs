using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DddEfteling.Rides.Boundaries;
using DddEfteling.Rides.Boundary;
using DddEfteling.Rides.Controls;
using DddEfteling.Shared.Boundary;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DddEfteling.Rides
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IEventConsumer, EventConsumer>();
            services.AddSingleton<IEventProducer, EventProducer>();
            services.AddSingleton<IEmployeeClient, EmployeeClient>();
            services.AddSingleton<IVisitorClient, VisitorClient>();
            services.AddSingleton<IRideControl, RideControl>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
