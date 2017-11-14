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
using TruststampCore.Interfaces;
using UnitTest.TruststampCore.Mocks;

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

            services.AddTransient<IConfiguration>(p => {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json.config", optional: true).Build();
                config["blockchain"] = "btctest"; // Use bitcoin test net
                config["btctest_fundingkey"] = "cMcGZkth7ufvQC59NSTSCTpepSxXbig9JfhCYJtn9RppU4DXx4cy"; // btc test net WIF key 
                return config;
                });


            services.AddTransient<IBlockingWorkflowStep, BlockingWorkflowStep>();
            services.AddTransient<IBlockchainRepository, BlockchainRepositoryMock>();

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
