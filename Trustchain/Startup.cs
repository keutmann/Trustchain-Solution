using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using TrustchainCore.Extensions;
using TrustgraphCore.Extensions;
using TrustchainCore.Repository;
using Microsoft.EntityFrameworkCore;


namespace Trustchain
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
            services.AddDbContext<TrustDBContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("TrustDB"), b => b.MigrationsAssembly("TrustchainCore"))); 
                //options.UseSqlite("Filename=./trust.db", b => b.MigrationsAssembly("TrustchainCore"))); 

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Trustchain API",
                    Version = "v1",
                    //Contact = new Contact
                    //{
                    //    Name = "Trustchain website",
                    //    Url = "/"
                    //},
                    //License = new License
                    //{
                    //    Name = "MIT License",
                    //    Url = "https://opensource.org/licenses/MIT"
                    //}
                });
            });

            services.TrustchainCore();
            services.TrustgraphCore();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

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
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });


            app.LoadGraph(); // Load the Trust Graph from Database
        }
    }
}
