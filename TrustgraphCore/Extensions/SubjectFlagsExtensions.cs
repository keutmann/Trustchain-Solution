using System.Runtime.CompilerServices;
using TrustgraphCore.Enumerations;

namespace TrustgraphCore.Extensions
{
    public static class SubjectFlagsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlagFast(this SubjectFlags flags, SubjectFlags checkflag)
        {
            return (flags & checkflag) == checkflag;
        }
    }
}
