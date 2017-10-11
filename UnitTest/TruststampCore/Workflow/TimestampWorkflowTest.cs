using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.TruststampCore.Workflow
{
    [TestClass]
    public class WorkflowTest
    {
        [TestMethod]
        public void SaveAndLoadWithSteps()
        {
            // Run the test against one instance of the context
            using (var context = InMemoryDataBase.Create())
            {

            }
        }
    }
}
