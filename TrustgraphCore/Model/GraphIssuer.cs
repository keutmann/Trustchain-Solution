using System;
using System.Runtime.InteropServices;

namespace TrustgraphCore.Model
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GraphIssuer
    {
        public byte[] Id;
        public GraphSubject[] Subjects;
    }
}
