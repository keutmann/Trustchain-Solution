using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TrustchainCore.Services;
using TruststampCore.Workflows;

namespace UnitTest.TruststampCore.Workflow
{
    [TestClass]
    public class TimestampWorkflowTest : StartupTest
    {
        [TestMethod]
        public void Create()
        {
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampWorkflow>(null);
            Assert.IsNotNull(workflow);
            workflow.CurrentStepIndex = 1;

            var container = workflowService.CreateWorkflowContainer(workflow);
            Assert.IsNotNull(container);
            Console.WriteLine(container.Data);

            var workflow2 = workflowService.Create<TimestampWorkflow>(container);
            Assert.IsNotNull(workflow2);
            Assert.AreEqual(workflow.Steps.Count, workflow2.Steps.Count);
            Assert.AreEqual(workflow.CurrentStepIndex, workflow2.CurrentStepIndex);


        }
    }
}
