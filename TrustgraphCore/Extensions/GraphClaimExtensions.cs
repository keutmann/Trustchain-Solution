using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TrustgraphCore.Model;

namespace TrustgraphCore.Extensions
{
    public static class GraphClaimExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exist(this Dictionary<long, int> claims, int scope, int type)
        {
            var subjectClaimIndex = new SubjectClaimIndex { Scope = scope, Type = type };
            return claims.ContainsKey(subjectClaimIndex.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Ensure(this Dictionary<long, int> claims, int scope, int type, int claimIndex)
        {
            var subjectClaimIndex = new SubjectClaimIndex { Scope = scope, Type = type };
            if (claims.ContainsKey(subjectClaimIndex.Value))
                return true;

            claims[subjectClaimIndex.Value] = claimIndex;
            return true;
        }

    }
}
