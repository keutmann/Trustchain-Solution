using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace TrustchainCore.Collections.Generic
{

    public static class ByteComparer
    {
        private class StandardComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[] left, byte[] right)
            {
                if (left == null || right == null)
                    return left == right;

                if (ReferenceEquals(left, right))
                    return true;

                if (left.Length != right.Length)
                    return false;

                int index = 0;
                while (index < left.Length)
                {
                    if (left[index] != right[index])
                        return false;
                    index++;
                }
                return true;
            }

            public int GetHashCode(byte[] key)
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                return key.Sum(b => b);
            }
        }



        public static readonly IEqualityComparer<byte[]> Standard = new StandardComparer();
    }
}
