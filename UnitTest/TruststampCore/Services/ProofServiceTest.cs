using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TrustchainCore.Extensions;
using TruststampCore.Interfaces;

namespace UnitTest.TruststampCore.Services
{
    [TestClass]
    public class ProofServiceTest : StartupTest
    {
        [TestMethod]
        public void AddProof()
        {
            var timestampSynchronizationService = ServiceProvider.GetRequiredService<ITimestampSynchronizationService>();
            timestampSynchronizationService.CurrentWorkflowID = 2;
            var proofService = ServiceProvider.GetRequiredService<IProofService>();

            var source = Guid.NewGuid().ToByteArray();
            var entity = proofService.AddProof(source);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.ID > 0);
            Assert.IsTrue(entity.WorkflowID == timestampSynchronizationService.CurrentWorkflowID);
            Assert.IsTrue(entity.Registered > DateTime.MinValue);
            Assert.IsTrue(source.Equals(entity.Source));
        }

        [TestMethod]
        public void GetProof()
        {
            var timestampSynchronizationService = ServiceProvider.GetRequiredService<ITimestampSynchronizationService>();
            timestampSynchronizationService.CurrentWorkflowID = 2;
            var proofService = ServiceProvider.GetRequiredService<IProofService>();
            var source = Guid.NewGuid().ToByteArray();
            var addEntity = proofService.AddProof(source);

            var getEntity = proofService.GetProof(source);
            Assert.AreEqual(addEntity.ID, getEntity.ID);
            Assert.AreEqual(addEntity.WorkflowID, getEntity.WorkflowID);
            Assert.IsTrue(addEntity.Source.Equals(getEntity.Source));
            Assert.AreEqual(addEntity.Registered, getEntity.Registered);
        }

    }
}
