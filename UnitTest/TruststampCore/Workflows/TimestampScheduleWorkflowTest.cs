﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using TrustchainCore.Enumerations;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Services;
using TruststampCore.Workflows;

namespace UnitTest.TruststampCore.Workflows
{
    [TestClass]
    public class TimestampScheduleWorkflowTest : StartupTest
    {
        [TestMethod]
        public void Serialize()
        {
            

            var resolver = ServiceProvider.GetService<IContractResolver>();
            var settings = new JsonSerializerSettings
            {
                ContractResolver = resolver,
                TypeNameHandling = TypeNameHandling.Auto
            };
            settings.Converters.Add(new DICustomConverter<TimestampScheduleWorkflow>(ServiceProvider));

            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var timestampSynchronizationService = ServiceProvider.GetRequiredService<ITimestampSynchronizationService>();
            var workflow = new TimestampScheduleWorkflow(workflowService);
            workflow.Initialize();

            var data = JsonConvert.SerializeObject(workflow, settings);
            Console.WriteLine(data);
            var wf2 = JsonConvert.DeserializeObject<TimestampWorkflow>(data, settings);
            Assert.AreEqual(workflow.CurrentStepIndex, wf2.CurrentStepIndex);
        }


        [TestMethod]
        public void ExecuteWithNoProof()
        {
            var timestampSynchronizationService = ServiceProvider.GetRequiredService<ITimestampSynchronizationService>();
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampScheduleWorkflow>();
            workflow.Execute().Wait();

            Assert.AreEqual(WorkflowStatusType.Running.ToString(), workflow.State);
            Assert.IsTrue(timestampSynchronizationService.CurrentWorkflowID == 0);
            var saveCurrentID = timestampSynchronizationService.CurrentWorkflowID;

            workflow.Execute().Wait();
            Assert.AreEqual(saveCurrentID, timestampSynchronizationService.CurrentWorkflowID);
            Assert.AreEqual(WorkflowStatusType.Running.ToString(), workflow.State);
        }

        [TestMethod]
        public void ExecuteWithProof()
        {
            var timestampSynchronizationService = ServiceProvider.GetRequiredService<ITimestampSynchronizationService>();
            var workflowService = ServiceProvider.GetRequiredService<IWorkflowService>();
            var workflow = workflowService.Create<TimestampScheduleWorkflow>();
            workflow.Execute().Wait();

            Assert.AreEqual(WorkflowStatusType.Running.ToString(), workflow.State);
            Assert.IsTrue(timestampSynchronizationService.CurrentWorkflowID == 0);
            var saveCurrentID = timestampSynchronizationService.CurrentWorkflowID;

            var proofService = ServiceProvider.GetRequiredService<IProofService>();
            proofService.AddProof(Guid.NewGuid().ToByteArray());
            workflow.NextExecution = DateTime.MinValue;

            workflow.Execute().Wait();
            Assert.AreNotEqual(saveCurrentID, timestampSynchronizationService.CurrentWorkflowID);
            Assert.AreEqual(WorkflowStatusType.Running.ToString(), workflow.State);
        }

    }
}