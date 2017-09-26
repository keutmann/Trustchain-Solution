using System.Runtime.InteropServices;

namespace TrustgraphCore.Model
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VisitItem
    {
        public int ParentIndex;
        public int SubjectIndex;
        //public int Cost;

        public VisitItem(int parentIndex, int subjectIndex)
        {
            ParentIndex = parentIndex;
            SubjectIndex = subjectIndex;
            //Cost = cost;
        }
    }
}
