using System.Linq;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TrustchainCore.Repository;

namespace TrustchainCore.Services
{
    public class KeyValueService : IKeyValueService
    {
        public TrustDBContext DBContext { get; }

        public KeyValueService(TrustDBContext trustDBContext)
        {
            DBContext = trustDBContext;
        }

        public void Set(string key, byte[] value)
        {
            var result = DBContext.KeyValues.FirstOrDefault(p => p.Key.Equals(key));
            if (result != null)
            {
                result.Value = value;
            }
            else
            {
                result = new KeyValue();
                result.Key = key;
                result.Value = value;
                DBContext.KeyValues.Add(result);
            }

            DBContext.SaveChanges();
        }

        public byte[] Get(string key)
        {
            var result = DBContext.KeyValues.FirstOrDefault(p => p.Key.Equals(key));
            if(result != null)
                return result.Value;
            return null;
        }

        
    }
}
