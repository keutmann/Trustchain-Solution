using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trustchain;
using TrustchainCore.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using UnitTest.TrustchainCore.Workflows;

namespace UnitTest
{
    public class StartupTest : Startup, IDisposable
    {
        public IServiceProvider  ServiceProvider { get; set; }
        public IServiceScope ServiceScope { get; set; }

        [TestInitialize]
        public void Init()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            services.AddTransient<IConfiguration>(p => new ConfigurationBuilder().Build());
            services.AddTransient<IBlockingWorkflowStep, BlockingWorkflowStep>();

            ServiceScope = services.BuildServiceProvider(false).CreateScope();
            ServiceProvider = ServiceScope.ServiceProvider;
            ILoggerFactory loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddConsole();
        }

        [TestCleanup]
        public void Cleanup()
        {
            ServiceScope.Dispose();
        }

        public StartupTest() : base(null)
        {
        }

        public StartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureDbContext(IServiceCollection services)
        {
            services.AddDbContext<TrustDBContext>(options =>
                options.UseInMemoryDatabase("UnitTest")
            );
            
        }

        public override void ConfigureTimers(IApplicationBuilder app)
        {
            // Do not create timers here!
        }

        public static StartupTest CreateStartupTest()
        {
            var startup = new StartupTest(null);
            startup.Init();
            return startup;
        }

        public static TrustDBContext CreateDBContext()
        {
            var options = new DbContextOptionsBuilder<TrustDBContext>()
                    .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                    .Options;

            // Run the test against one instance of the context
            return new TrustDBContext(options);
        }


        public void Dispose()
        {
            if (ServiceScope != null)
                ServiceScope.Dispose();
        }
    }
}
