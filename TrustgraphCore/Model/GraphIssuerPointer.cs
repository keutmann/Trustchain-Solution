using System;
using System.Runtime.InteropServices;

namespace TrustgraphCore.Model
{
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class GraphIssuerPointer
    {
        public ulong Visited;
        public GraphSubjectPointer[] Subjects;
    }
}
