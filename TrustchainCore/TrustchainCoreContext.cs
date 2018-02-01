using System;
using System.Collections.Generic;
using System.Text;
using TrustchainCore.Interfaces;
using TrustchainCore.Strategy;

namespace TrustchainCore
{
    /// <summary>
    /// Centralize the init of multiple services and strategies.
    /// </summary>
    public class TrustchainCoreContext
    {
        public static IDerivationStrategy DerivationStrategy = new DerivationBTCPKH();
    }
}
