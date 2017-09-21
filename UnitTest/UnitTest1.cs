using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TrustchainCore.Model;
using TrustchainCore.Repository;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // In-memory database only exists while the connection is open
            //var connection = new SqliteConnection("DataSource=:memory:");
            //connection.Open();

            var options = new DbContextOptionsBuilder<TrustDBContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

            // Run the test against one instance of the context
            using (var context = new TrustDBContext(options))
            {
                var package = new PackageModel();
                package.Trust = new List<TrustModel>();
                var trust = new TrustModel();
                trust.Subjects = new List<SubjectModel>()  {
                    new SubjectModel {
                        Scope = "Test"
                    }
                };
                package.Trust.Add(trust);


                context.Package.Add(package);
                context.SaveChanges();
                //var service = new BlogService(context);
                //service.Add("http://sample.com");
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new TrustDBContext(options))
            {
                var task = context.Package
                    .Include(c => c.Trust)
                        .ThenInclude(c=> c.Subjects)
                    .AsNoTracking().ToListAsync();
                
                task.Wait();

                Assert.AreEqual(1, task.Result.Count);
                Assert.IsNotNull(task.Result[0].Trust);
                Assert.IsNotNull(task.Result[0].Trust[0]);
                Assert.IsNotNull(task.Result[0].Trust[0].Subjects[0]);
                Assert.AreEqual("Test", task.Result[0].Trust[0].Subjects[0].Scope);
            }
        }
    }
}
