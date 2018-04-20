using System;
using System.Collections.Generic;
using System.Text;

namespace TrustchainCore.Interfaces
{
    public interface ITimestamp
    {
        byte[] Source { get; set; }
        byte[] Receipt { get; set; }
        long Registered { get; set; }
    }
}
