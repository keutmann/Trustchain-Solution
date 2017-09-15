using System;
using System.Collections.Generic;
using System.Text;

namespace TrustgraphCore.Services
{
    public class SHA256Service : ICryptoAlgoService
    {
        public int Length { get; }

        public SHA256Service()
        {
            Length = 20;
        }
    }
}
