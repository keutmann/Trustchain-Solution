using Newtonsoft.Json.Linq;

namespace TruststampCore.Services
{
    public class Proof 
    {


        public Proof() 
        {
        }

        public JObject Add(string id)
        {
            //var idc =  IDContainer.Parse(id);
            
            //var item = DB.ProofTable.GetByHash(idc.Hash);
            //if (item != null)
            //    return item;

            //item = DB.ProofTable.NewItem(idc.Hash, null, Batch.GetCurrentPartition(), DateTime.Now);
            //DB.ProofTable.Add(item);

            return null;
        }

        public JObject Get(string id)
        {
            //var idc = IDContainer.Parse(id);
            
            //var item = DB.ProofTable.GetByHash(idc.Hash);
            //if (item != null)
            //    return item;

            return null;
        }

        //public JArray UnprocessedPartitions()
        //{
        //    return DB.Proof.GetUnprocessed();
        //}



    }
}
