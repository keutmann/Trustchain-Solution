using System;
using System.Runtime.InteropServices;

namespace TrustgraphCore.Model
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SubjectClaimIndex
    {
        [FieldOffset(0)]
        public Int64 Value;
        [FieldOffset(0)]
        public Int32 Scope;
        [FieldOffset(4)]
        public Int32 Index;

        public SubjectClaimIndex(Int64 value)
        {
            Scope = 0;
            Index = 0;
            Value = value;
        }

        public SubjectClaimIndex(Int32 scope, Int32 index)
        {
            Value = 0;
            Scope = scope;
            Index = index;
        }
    }
}
