using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using TrustchainCore.Extensions;
using TrustgraphCore.Extensions;
using TruststampCore.Extensions;
using TrustchainCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using TrustchainCore.Attributes;

namespace Trustchain
{
    public class Startup
    {
        private IServiceCollection _services;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDbContext(services);
            
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Trustchain API",
                    Version = "v1",
                });
            });


            services.TrustchainCore();
            services.TrustgraphCore();
            services.TruststrampCore();

            _services = services;

        }

        public virtual void ConfigureDbContext(IServiceCollection services)
        {
            services.AddDbContext<TrustDBContext>(options =>
                //options.UseSqlite(Configuration.GetConnectionString("TrustDB"), b => b.MigrationsAssembly("TrustchainCore"))); 
                options.UseSqlite("Filename=./trust.db", b => b.MigrationsAssembly("TrustchainCore")));
                //options.UseSqlite("Filename=./trust.db"));

        }

        public virtual void ConfigureTimers(IApplicationBuilder app)
        {
            app.Trustchain();
        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();

                loggerFactory.AddConsole();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            //app.LoadGraph(); // Load the Trust Graph from Database
            app.Truststamp();
            ConfigureTimers(app);
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "stamp",
                //    template: "v1/stamp/{blockchain}/{action=Index}/{source?}");

                routes.MapRoute(
                    name: "default",
                    template: "v1/{controller}/{action=Index}/{id?}");
            });


            app.AllServices(_services);
        }

    }
}
