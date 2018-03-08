using System.IO;
using TrustchainCore.Extensions;
using TrustchainCore.Model;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Strategy
{
    public class TrustBinary : ITrustBinary
    {
        public TrustBinary()
        {
        }

        public byte[] GetIssuerBinary(Trust trust)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteString(trust.IssuerScript.ToLowerSafe());
                ms.WriteBytes(trust.IssuerAddress);
                ms.WriteString(trust.SubjectScript.ToLowerSafe());
                ms.WriteBytes(trust.SubjectAddress);
                ms.WriteString(trust.Type);
                ms.WriteString(trust.Scope);
                ms.WriteString(trust.Attributes);
                ms.WriteInteger(trust.Cost);
                ms.WriteInteger(trust.Activate);
                ms.WriteInteger(trust.Expire);
                //ms.WriteString(trust.Note);

                return ms.ToArray();
            }
        }
    }
}
