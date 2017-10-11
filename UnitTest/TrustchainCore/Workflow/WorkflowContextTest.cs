using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Trustchain;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using TrustchainCore.Services;
using TrustchainCore.Workflows;

namespace UnitTest.TrustchainCore.Workflow
{
    [TestClass]
    public class WorkflowContextTest
    {
        [TestMethod]
        public void Test1()
        {
            // http://geeklearning.io/a-different-approach-to-test-your-asp-net-core-application/
            //var workflowService = new WorkflowService();
            //var context = new WorkflowContext(workflowService);
            //var webHostBuilder = new WebHostBuilder();
            ////webHostBuilder.ConfigureServices(
            ////    s => s.AddSingleton<IStartupConfigurationService, TestStartupConfigurationService <[DBCONTEXT_TYPE] >> ());
            //webHostBuilder.UseStartup<Startup>();
            

            //var testServer = new TestServer(webHostBuilder);
            //var result = testServer.CreateClient().GetAsync("/api/trust?proof=abc").Result;
            //result.EnsureSuccessStatusCode();

            var services = new ServiceCollection();
            var ss = new Startup(null);
            ss.ConfigureServices(services);
            services.AddDbContext<TrustDBContext>(options => 
                options.UseInMemoryDatabase("HANS")
            );
                        

            var serviceProvider = services.BuildServiceProvider(false);
            var db = serviceProvider.GetService<TrustDBContext>();
            var t = db.Database.ProviderName;

            var workflowService = serviceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<WorkflowContext>(null);

            var entity = workflowService.CreateWorkflowEntity(workflow);

            db.Workflows.Add(entity);
            db.SaveChanges();

            var options2 = new DbContextOptionsBuilder<TrustDBContext>()
                    .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                    .Options;

            // Run the test against one instance of the context
            using (var context = new TrustDBContext(options2))
            {
                context.Workflows.Add(entity);
                context.SaveChanges();
            }


            //workflow.ID = workflowService.Save(workflow);

            //var entity = workflowService.DBService.Workflows.FirstOrDefault(p => p.ID == workflow.ID);

            //var loadedWF = workflowService.Create<WorkflowContext>(entity);

            //Assert.AreEqual(workflow.ID, loadedWF.ID);
            //JsonSerializerSettings settings = new JsonSerializerSettings
            //{
            //    ContractResolver = serviceProvider.GetRequiredService<IContractResolver>()
            //};

            //var data = JsonConvert.SerializeObject(workflow, settings);
            //var entity = new WorkflowEntity();
            //entity.Data = data;
        }
    }
}
