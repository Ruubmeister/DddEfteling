using DddEfteling.Park.Boundaries;
using DddEfteling.Park.Controls;
using DddEfteling.Shared.Controls;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace DddEfteling.Park
{
    public class Startup
    {
        private readonly string DefaultCorsPolicy = "_defaultCorsPolicy";

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

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
                    builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEntranceControl entranceControl,
            IEventConsumer eventConsumer)
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

            _ = Task.Run(() => eventConsumer.Listen());
            entranceControl.OpenPark();
        }
    }
}
