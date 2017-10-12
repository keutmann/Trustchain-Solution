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
using TrustchainCore.Enumerations;

namespace UnitTest.TrustchainCore.Workflow
{
    [TestClass]
    public class WorkflowServiceTest : StartupTest
    {
        
        [TestMethod]
        public void Create()
        {
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<WorkflowContext>(null);
            Assert.IsNotNull(workflow);
            workflow.CurrentStepIndex = 1;

            var container = workflowService.CreateWorkflowContainer(workflow);
            Assert.IsNotNull(container);

            var workflow2 = workflowService.Create<WorkflowContext>(container);
            Assert.IsNotNull(workflow2);
            Assert.AreEqual(workflow.CurrentStepIndex, workflow2.CurrentStepIndex);
            //workflow.ID = workflowService.Save(workflow);

            //var db = ServiceProvider.GetService<TrustDBContext>();
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

        [TestMethod]
        public void CreateWorkflowContainer()
        {
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<WorkflowContext>();
            Assert.IsNotNull(workflow);
            var container = workflowService.CreateWorkflowContainer(workflow);
            Assert.IsNotNull(container);
            Assert.IsNotNull(container.Data);
            Assert.IsTrue(container.Data.Length > 0);
            Assert.AreEqual(WorkflowStatus.New.ToString(), container.State);
        }

        [TestMethod]
        public void Save()
        {
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<WorkflowContext>();
            Assert.IsNotNull(workflow);

            var id = workflowService.Save(workflow);
            Assert.AreEqual(id, workflow.ID);
        }


        //[TestMethod]
        //public void Execute()
        //{

        //}

    }
}
