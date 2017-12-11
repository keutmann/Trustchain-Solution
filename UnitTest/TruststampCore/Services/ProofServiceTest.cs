using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TrustchainCore.Extensions;
using TruststampCore.Interfaces;

namespace UnitTest.TruststampCore.Services
{
    [TestClass]
    public class ProofServiceTest : StartupMock
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
            Assert.IsTrue(entity.DatabaseID > 0);
            Assert.IsTrue(entity.WorkflowID == timestampSynchronizationService.CurrentWorkflowID);
            Assert.IsTrue(entity.Registered > DateTime.MinValue.ToUnixTime());
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
            Assert.AreEqual(addEntity.DatabaseID, getEntity.DatabaseID);
            Assert.AreEqual(addEntity.WorkflowID, getEntity.WorkflowID);
            Assert.IsTrue(addEntity.Source.Equals(getEntity.Source));
            Assert.AreEqual(addEntity.Registered, getEntity.Registered);
            Assert.IsTrue(getEntity.Registered > DateTime.MinValue.ToUnixTime());
        }

    }
}
