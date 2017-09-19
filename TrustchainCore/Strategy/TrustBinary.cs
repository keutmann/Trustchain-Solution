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
                var issuer = trust.Issuer;
                ms.WriteBytes(issuer.IssuerId);
                foreach (var subject in issuer.Subjects)
                {
                    ms.WriteBytes(subject.SubjectId);
                    ms.WriteString(subject.IdType);

                    ms.WriteString(subject.Claim);
                    //foreach (JProperty prop in subject.Claim.Children().OfType<JProperty>())
                    //{
                    //    ms.WriteString(prop.Name.ToLower());
                    //    ms.WriteString(prop.Value.ToStringValue());
                    //}
                    //foreach (DictionaryEntry claim in subject.Claim)
                    //{
                    //    ms.WriteString(claim.Key);
                    //    ms.WriteString(claim.Value);
                    //}
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
