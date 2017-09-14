using System;

namespace TrustchainCore.IOC
{
    public class IOCAttribute : Attribute
    {
        public IOCLifeCycleType LifeCycle { get; set; }
    }
}
