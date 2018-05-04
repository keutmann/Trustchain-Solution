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
using Microsoft.Extensions.FileProviders;
using System.IO;
using System;

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

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = false;
            });

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
            //app.Trustchain(_services);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();

                //loggerFactory.AddConsole();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.Graph(); // Load the Trust Graph from Database
            app.Truststamp();
            ConfigureTimers(app);

            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
            //    RequestPath = "/Resources",


            //});

            //app.UseDirectoryBrowser(new DirectoryBrowserOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
            //    RequestPath = "/Resources"
            //});

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseSession();

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "stamp",
                //    template: "v1/stamp/{blockchain}/{action=Index}/{source?}");
                //routes.MapRoute("Proof.htm");

                routes.MapRoute(
                    name: "default",
                    template: "v1/{controller}/{action=Index}/{id?}");
            });


            app.AllServices(_services);
        }

    }
}
