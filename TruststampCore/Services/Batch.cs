using Newtonsoft.Json.Linq;
using System;


namespace TrustStampCore.Services
{
    public class Batch 
    {
        public static Func<string> PartitionMethod = DefaultPartition;

        public static string DefaultPartition()
        {
            return GetPartition(DateTime.Now);
        } 

        public static string GetPartition(DateTime datetime)
        {
            //return datetime.ToString( App.Config["partition"].ToStringValue("yyyyMMddhh0000"));
            return datetime.ToString("yyyyMMddhh0000");
        }

        public static string GetCurrentPartition()
        {
            return PartitionMethod();
        }


        public Batch()
        {
        }

        public JObject Get(string partition)
        {
            //return DB.BatchTable.GetByPartition(partition);
            return null;
        }

        public void Process()
        {
            EnsureNewBatchs(); // Make sure to create new Batchs
            ProcessBatchs(); // 
        }

        protected void ProcessBatchs()
        {
            //var batchs = DB.BatchTable.GetActive();
            var batchs = new JArray();

            var engine = new WorkflowContext(batchs);
            engine.Execute();
        }

        public void EnsureNewBatchs()
        {
            var currentPartition = GetCurrentPartition();// current partition snapshot
            //var partitions = DB.ProofTable.GetUnprocessedPartitions(currentPartition); // partitions are ordered!
            //var partitions =


            //foreach (var item in partitions)
            //{
            //    //var partition = item["partition"].ToString();
            //    DB.BatchTable.Ensure(partition);
            //}
        }



    }
}
