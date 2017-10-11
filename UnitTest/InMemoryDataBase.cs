using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Repository;

namespace UnitTest
{
    public class InMemoryDataBase
    {
        public static TrustDBContext Create()
        {
            var options = new DbContextOptionsBuilder<TrustDBContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;

            // Run the test against one instance of the context
            return new TrustDBContext(options);
        }
    }
}
