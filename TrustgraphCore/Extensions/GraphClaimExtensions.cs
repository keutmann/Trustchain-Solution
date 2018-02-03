using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TrustgraphCore.Model;

namespace TrustgraphCore.Extensions
{
    public static class GraphClaimExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exist(this Dictionary<long, GraphClaimPointer> claims, int scope, int index)
        {
            var subjectClaimIndex = new SubjectClaimIndex { Scope = scope, Index = index };
            return claims.ContainsKey(subjectClaimIndex.Value);
        }
    }
}
