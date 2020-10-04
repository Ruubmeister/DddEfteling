using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using DddEfteling.Shared.Controls;
using DddEfteling.Park.Boundaries;
using DddEfteling.Park.Controls;

namespace DddEfteling.Park
{
    public class Startup
    {
        readonly string DefaultCorsPolicy = "_defaultCorsPolicy";

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
            services.AddSingleton<IEventConsumer, EventConsumer>();
            services.AddRazorPages();
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
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(DefaultCorsPolicy);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
