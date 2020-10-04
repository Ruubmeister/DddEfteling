using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DddEfteling.Shared.Boundary;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Boundary;
using DddEfteling.Visitors.Controls;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DddEfteling.Visitors
{
    public class Startup
    {
        readonly string DefaultCorsPolicy = "_defaultCorsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IEventProducer, EventProducer>();
            services.AddSingleton<IEventConsumer, EventConsumer>();
            services.AddSingleton<IRideClient, RideClient>();
            services.AddSingleton<IFairyTaleClient, FairyTaleClient>();
            services.AddSingleton<IVisitorControl, VisitorControl>();
            services.AddMediatR(typeof(Startup));
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
