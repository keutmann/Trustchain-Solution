using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrustchainCore.Services;
using TrustchainCore.Workflows;
using TrustchainCore.Enumerations;
using TrustchainCore.Interfaces;
using System.Collections.Generic;
using UnitTest.TrustchainCore.Workflows;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace UnitTest.TrustchainCore.Workflow
{
    [TestClass]
    public class WorkflowServiceTest : StartupTest
    {

        [TestMethod]
        public void Create()
        {
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<WorkflowContext>();
            Assert.IsNotNull(workflow);

            var container = workflowService.CreateWorkflowContainer(workflow);
            Assert.IsNotNull(container);

            var workflow2 = workflowService.Create(container);
            Assert.IsNotNull(workflow2);
            Assert.AreEqual(workflow.CurrentStepIndex, workflow2.CurrentStepIndex);
        }


        [TestMethod]
        public void CreateGeneric()
        {
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<WorkflowContext>();
            Assert.IsNotNull(workflow);
            workflow.CurrentStepIndex = 1;

            var container = workflowService.CreateWorkflowContainer(workflow);
            Assert.IsNotNull(container);

            var workflow2 = workflowService.Create(container);
            Assert.IsNotNull(workflow2);
            Assert.AreEqual(workflow.CurrentStepIndex, workflow2.CurrentStepIndex);
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
            Assert.AreEqual(WorkflowStatusType.New.ToString(), container.State);
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


        [TestMethod]
        public void ExecuteOne()
        {
            var executionSynchronizationService = ServiceProvider.GetService<IExecutionSynchronizationService>();

            var list = new List<IWorkflowContext>();
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<WorkflowContext>();
            Assert.IsNotNull(workflow);
            list.Add(workflow);

            workflowService.Execute(list);
            Assert.AreEqual(WorkflowStatusType.Finished.ToString(), workflow.State);
            Assert.AreEqual(0, executionSynchronizationService.Workflows.Count);
        }

        [TestMethod]
        public void ExecuteMany()
        {
            var executionSynchronizationService = ServiceProvider.GetService<IExecutionSynchronizationService>();

            var list = new List<IWorkflowContext>();
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            for (int i = 0; i < 10; i++)
            {
                var workflow = workflowService.Create<WorkflowContext>();
                workflow.ID = i;
                list.Add(workflow);
            }

            workflowService.Execute(list);
            Assert.AreEqual(0, executionSynchronizationService.Workflows.Count);

            foreach (var item in list)
            {
                Assert.AreEqual(WorkflowStatusType.Finished.ToString(), item.State);
            }
        }

        [TestMethod]
        public void GetRunningWorkflows()
        {
            // Setup
            var count = 10;
            var list = new List<IWorkflowContext>();
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            for (int i = 0; i < count; i++)
            {
                var workflow = workflowService.Create<WorkflowContext>();
                var id = workflowService.Save(workflow);
                list.Add(workflow);
            }
            var wf = workflowService.Create<WorkflowContext>();
            wf.State = WorkflowStatusType.Finished.ToString();
            workflowService.Save(wf);

            wf = workflowService.Create<WorkflowContext>();
            wf.State = WorkflowStatusType.Failed.ToString();
            workflowService.Save(wf);

            // Execute
            var results = workflowService.GetRunningWorkflows();

            // Verify
            Assert.AreEqual(count, results.Count);
        }


        [TestMethod]
        public void RunWorkflows()
        {

            var logger = ServiceProvider.GetRequiredService<ILogger<WorkflowServiceTest>>();

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<WorkflowContext>();

            var step = new BlockingWorkflowStep();
            workflow.Steps.Add(step);
            var id = workflowService.Save(workflow);

            logger.LogInformation("Running RunWorkflows");
            workflowService.RunWorkflows();

            //logger.LogInformation("Running RunWorkflows second time");
            //workflowService.RunWorkflows();


            Task.Delay(5 * 1000).Wait();



            //    Assert.IsNotNull(workflow);
            //    list.Add(workflow);

            //    workflowService.Execute(list);
            //    Assert.AreEqual(WorkflowStatusType.Finished.ToString(), workflow.State);
            //    workflow.State = WorkflowStatusType.Running.ToString();


            //    var workflowService2 = ServiceProvider.GetRequiredService<IWorkflowService>();
            //    workflowService2.Execute(list);

        }


    }
}
