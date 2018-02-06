using System;
using System.Collections.Generic;
using System.Text;

namespace TrustchainCore.Interfaces
{
    public interface IGraphClaim
    {
        string StringID();
        byte[] ByteID();
    }
}
