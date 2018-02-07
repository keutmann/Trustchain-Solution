using NBitcoin.Crypto;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TrustchainCore.Interfaces;

namespace TrustgraphCore.Model
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GraphClaim //: IGraphClaim
    {
        public int Index;
        public short Cost;  // cost of following the trust, lower the better
        public int Scope; // scope of the trust
        public int Type; // Type of the trust
        public int Data; // Claims 
        public int Note;
        //public int Activate; // When to begin consider the trust. This will be moved to an external event, do not check every time traversing the Graph
        //public int Expire;    // When the trust expire, This will be moved to an external event

        public string ID()
        {
            return $"T:{Type}:{Scope}:{Data}:{Note}";
        }

        //public static byte[] ByteID(string id)
        //{
        //    var data = Encoding.UTF8.GetBytes(id);
        //    return Hashes.RIPEMD160(data, data.Length);
        //}



    }
}
