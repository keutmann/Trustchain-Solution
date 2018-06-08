using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrustchainCore.Builders;
using TrustchainCore.Extensions;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using TrustchainCore.Services;
using TruststampCore.Interfaces;
using TruststampCore.Workflows;
using UnitTest.TrustchainCore.Extensions;

namespace UnitTest.TrustchainCore.DBContext
{
    [TestClass]
    public class TrustModelTest : StartupMock
    {


        [TestMethod]
        public void SaveAndLoad()
        {
            var trustDBContext = ServiceProvider.GetRequiredService<TrustDBContext>();
            var timestampSource = Guid.NewGuid().ToByteArray();

            var builder = new TrustBuilder(ServiceProvider);
            builder.SetServer("testserver");
            builder.AddTrust("A", "B", TrustBuilder.BINARYTRUST_TC1, TrustBuilder.CreateBinaryTrustAttributes(true));
            builder.Build().Sign();

            var trust = builder.CurrentTrust;
            trust.Timestamps = new List<Timestamp>
            {
                new Timestamp
                {
                    Algorithm = "BTC-PKH",
                    Blockchain = "BTC",
                    Registered = DateTime.Now.ToUnixTime(),
                    Service = "Some url",
                    Source = timestampSource
                }
            };

            trustDBContext.Trusts.Add(trust);
            trustDBContext.SaveChanges();

            var loadTrust = trustDBContext.Trusts.AsNoTracking().FirstOrDefault(p => p.Id == trust.Id);

            Assert.IsNotNull(loadTrust);
            Assert.IsTrue(trust.Id.SequenceEqual(loadTrust.Id));
            Assert.IsTrue(trust.Timestamps.Count() > 0, "No timestamps in trust");
            Assert.IsTrue(trust.Timestamps[0].Source.SequenceEqual(timestampSource), "Timestamp source are not equal");

        }


    }
}
