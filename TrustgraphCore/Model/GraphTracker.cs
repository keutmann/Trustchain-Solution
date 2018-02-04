using System.Runtime.InteropServices;

namespace TrustgraphCore.Model
{
    /// <summary>
    /// Used to run though the Graph and track the path of search expantion. Enableds iterative free functions.
    /// </summary>
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class GraphTracker
    {

        public int SubjectKey;
        public GraphIssuerPointer Issuer;

        public GraphTracker(GraphIssuerPointer issuer)
        {
            SubjectKey = -1;
            Issuer = issuer;
        }
    }
}
