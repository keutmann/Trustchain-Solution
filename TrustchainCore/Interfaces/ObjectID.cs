using System;
using System.Collections.Generic;
using System.Text;

namespace TrustchainCore.Interfaces
{
    public interface ObjectID
    {
        string StringID();
        byte[] RIPEMD160();
    }
}
