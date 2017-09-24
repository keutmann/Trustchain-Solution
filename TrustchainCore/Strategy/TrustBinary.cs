using Newtonsoft.Json.Linq;
using System.Linq;
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

        public byte[] GetIssuerBinary(TrustModel trust)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteBytes(trust.IssuerId);
                ms.WriteString(trust.Name); // UTF8 - Not sure that name should be included in trust ID.
                foreach (var subject in trust.Subjects)
                {
                    ms.WriteBytes(subject.SubjectId);
                    ms.WriteString(subject.SubjectType);

                    ms.WriteString(subject.Claim); // UTF8
                    ms.WriteInteger(subject.Cost);
                    ms.WriteInteger(subject.Activate);
                    ms.WriteInteger(subject.Expire);
                    ms.WriteString(subject.Scope);
                }

                return ms.ToArray();
            }
        }
    }
}
