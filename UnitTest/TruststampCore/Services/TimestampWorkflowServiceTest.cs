using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Interfaces;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;
using TrustchainCore.Enumerations;
using TrustchainCore.Services;

namespace UnitTest.TruststampCore.Services
{
    [TestClass]
    public class TimestampWorkflowServiceTest : StartupMock
    {
        [TestMethod]
        public void EnsureTimestampScheduleWorkflow()
        {
            var timestampWorkflowService = ServiceProvider.GetRequiredService<ITimestampWorkflowService>();
            var trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();

            var noEntity = trustDBService.Workflows.FirstOrDefault(p => p.Type == typeof(TimestampScheduleWorkflow).FullName
                                             && (p.State == WorkflowStatusType.New.ToString()
                                             || p.State == WorkflowStatusType.Running.ToString()));

            Assert.IsNull(noEntity);
            timestampWorkflowService.EnsureTimestampScheduleWorkflow();
            var entity = trustDBService.Workflows.FirstOrDefault(p => p.Type == typeof(TimestampScheduleWorkflow).FullName
                                 && (p.State == WorkflowStatusType.New.ToString()
                                 || p.State == WorkflowStatusType.Running.ToString()));

            Assert.IsNotNull(entity);

            timestampWorkflowService.EnsureTimestampScheduleWorkflow();

            var count = trustDBService.Workflows.Count(p => p.Type == typeof(TimestampScheduleWorkflow).FullName
                                 && (p.State == WorkflowStatusType.New.ToString()
                                 || p.State == WorkflowStatusType.Running.ToString()));

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void EnsureTimestampWorkflow()
        {
            var timestampSynchronizationService = ServiceProvider.GetRequiredService<ITimestampSynchronizationService>();
            var timestampWorkflowService = ServiceProvider.GetRequiredService<ITimestampWorkflowService>();
            var trustDBService = ServiceProvider.GetRequiredService<ITrustDBService>();

            timestampWorkflowService.CreateTimestampWorkflow();

            var count = trustDBService.Workflows.Count(p => p.Type == typeof(TimestampWorkflow).FullName
                     && (p.State == WorkflowStatusType.New.ToString()
                     || p.State == WorkflowStatusType.Running.ToString()));

            Assert.AreEqual(1, count);
            Assert.IsTrue(timestampSynchronizationService.CurrentWorkflowID > 0);
        }
    }
}
