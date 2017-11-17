using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Reflection;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;

namespace UnitTest.TruststampCore.Workflows
{
    [TestClass]
    public class TimestampWorkflowTest : StartupMock
    {
        [TestMethod]
        public void Serialize()
        {
            var resolver = ServiceProvider.GetService<IContractResolver>();
            var settings = new JsonSerializerSettings
            {
                ContractResolver = resolver,
                TypeNameHandling = TypeNameHandling.Objects
            };

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var timestampSynchronizationService = ServiceProvider.GetRequiredService<ITimestampSynchronizationService>();
            var workflow = new TimestampWorkflow(workflowService, timestampSynchronizationService, ServiceProvider);
            workflow.Initialize();
            //workflow.CurrentStepIndex = 2;
            var merkleStep = (IMerkleStep)workflow.Steps[0];

            var reverseResolver = ServiceProvider.GetService<IContractReverseResolver>();
            var settings2 = new JsonSerializerSettings
            {
                ContractResolver = reverseResolver,
                TypeNameHandling = TypeNameHandling.Objects
            };
            var data = JsonConvert.SerializeObject(workflow, Formatting.Indented, settings2);
            Console.WriteLine(data);
            var wf2 = (TimestampWorkflow)JsonConvert.DeserializeObject(data, settings);
            merkleStep = (IMerkleStep)wf2.Steps[0];

            //Assert.AreEqual(workflow.CurrentStepIndex, wf2.CurrentStepIndex);
            //Assert.AreEqual(((IMerkleStep)workflow.Steps[0]).RootHash[0], ((IMerkleStep)wf2.Steps[0]).RootHash[0]);
        }


        [TestMethod]
        public void Create()
        {
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();
            Assert.IsNotNull(workflow);
            //workflow.CurrentStepIndex = 1;

            var container = workflowService.CreateWorkflowContainer(workflow);
            Assert.IsNotNull(container);
            Console.WriteLine(container.Data);

            var workflow2 = workflowService.Create(container);
            Assert.IsNotNull(workflow2);
            Assert.AreEqual(workflow.Steps.Count, workflow2.Steps.Count);
            //Assert.AreEqual(workflow.CurrentStepIndex, workflow2.CurrentStepIndex);
        }

        [TestMethod]
        public void Load()
        {
            var trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>(); 
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>();
            Assert.IsNotNull(workflow);
            //workflow.CurrentStepIndex = 2;
            //((IMerkleStep)workflow.Steps[0]).RootHash = new byte[] { 1 };
            var id = workflowService.Save(workflow);

            var container = trustDBService.Workflows.FirstOrDefault(p => p.ID == id);
            Assert.IsNotNull(container);
            Console.WriteLine(container.Data);

            var workflow2 = workflowService.Create(container);
            Assert.IsNotNull(workflow2);
            Assert.AreEqual(workflow.Steps.Count, workflow2.Steps.Count);
            //Assert.AreEqual(workflow.CurrentStepIndex, workflow2.CurrentStepIndex);
            //Assert.AreEqual(((IMerkleStep)workflow.Steps[0]).RootHash[0], ((IMerkleStep)workflow2.Steps[0]).RootHash[0]);
        }
    }
}
