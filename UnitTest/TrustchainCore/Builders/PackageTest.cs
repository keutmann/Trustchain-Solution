using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Text;
using TrustchainCore.Builders;
using TrustchainCore.Enumerations;
using TrustchainCore.Factories;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TrustchainCore.Strategy;
using UnitTest.TrustchainCore.Extensions;

namespace UnitTest.TrustchainCore.Builders
{
    [TestClass]
    public class PackageTest : StartupMock
    {
        //[TestMethod]
        //public void Build()
        //{
        //    var builder = new TrustBuilder(ServiceProvider);
        //    builder.SetServer("testserver");
        //    builder.AddTrust("testissuer1")
        //        .AddSubject("testsubject1", TrustBuilder.CreateFollowClaim())
        //        .AddSubject("testsubject2", TrustBuilder.CreateFollowClaim());
        //    builder.AddTrust("testissuer2", "testsubject1", TrustBuilder.CreateFollowClaim());
        //    builder.Build();
        //    builder.Sign();

        //    var schemaService = ServiceProvider.GetRequiredService<ITrustSchemaService>();

        //    //schemaService = new TrustSchemaService(ServiceProvider);

        //    var result = schemaService.Validate(builder.Package);

        //    Console.WriteLine(result.ToString());

        //    Assert.IsTrue(builder.Package.Trusts.Count > 0);
        //    Assert.AreEqual(0, result.Errors.Count);
        //    Assert.AreEqual(0, result.Warnings.Count);

        //    var content = JsonConvert.SerializeObject(builder.Package, Formatting.Indented);
        //    Console.WriteLine(content);
        //}
    }
}
