using System;
using System.Collections.Generic;
using System.Text;

namespace TrustchainCore.Interfaces
{
    public interface IProof
    {
        byte[] Source { get; set; }
        byte[] Receipt { get; set; }
    }
}
