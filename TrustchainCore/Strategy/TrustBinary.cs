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
                ms.WriteString(trust.Issuer.Script.ToLower());
                ms.WriteBytes(trust.Issuer.Address);
                //ms.WriteString(trust.Name); // UTF8 - Not sure that name should be included in trust ID.
                foreach (var subject in trust.Subjects)
                {
                    ms.WriteBytes(subject.Address);
                    ms.WriteString(subject.Type);
                    ms.WriteString(subject.Alias);
                    
                    foreach (int index in subject.ClaimIndexs)
                    {
                        ms.WriteInteger(index);
                    }
                }

                foreach (var claim in trust.Claims)
                {
                    ms.WriteString(claim.Data); // UTF8
                    ms.WriteInteger(claim.Cost);
                    ms.WriteInteger(claim.Activate);
                    ms.WriteInteger(claim.Expire);
                    ms.WriteString(claim.Scope);
                }

                return ms.ToArray();
            }
        }
    }
}
