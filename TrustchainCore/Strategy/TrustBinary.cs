﻿using System.IO;
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
                if (trust.Issuer != null)
                {
                    ms.WriteString(trust.Issuer.Type.ToLowerSafe());
                    ms.WriteBytes(trust.Issuer.Address);
                }

                if (trust.Subject != null)
                {
                    ms.WriteString(trust.Subject.Type.ToLowerSafe());
                    ms.WriteBytes(trust.Subject.Address);
                }

                ms.WriteString(trust.Type.ToLowerSafe());
                ms.WriteString(trust.Claim);

                if (trust.Scope != null)
                {
                    ms.WriteString(trust.Scope.Type.ToLowerSafe());
                    ms.WriteString(trust.Scope.Value);
                }

                ms.WriteInteger(trust.Created);
                ms.WriteInteger(trust.Cost);
                ms.WriteInteger(trust.Activate);
                ms.WriteInteger(trust.Expire);

                return ms.ToArray();
            }
        }
    }
}
