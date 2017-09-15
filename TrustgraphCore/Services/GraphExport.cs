using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Services;

namespace TrustgraphCore.Service
{
    public class GraphExport : IGraphExport
    {
        private IGraphModelService Context;

        public GraphExport(IGraphModelService context)
        {
            Context = context;
        }

        public PackageModel GetFullGraph()
        {
            var package = new PackageModel();
            package.Trust = new List<TrustModel>();

            foreach (var address in Context.Graph.Address)
            {
                var issuer = new IssuerModel();
                issuer.Id = address.Id;
                
                var subjects = new List<SubjectModel>();
                if (address.Edges != null)
                {
                    foreach (var edge in address.Edges)
                    {
                        var child = new SubjectModel();
                        Context.InitSubjectModel(child, edge);
                        subjects.Add(child);
                    }
                }
                if(subjects.Count > 0)
                    issuer.Subjects = subjects.ToArray();

                var trust = new TrustModel();
                trust.Issuer = issuer;
                package.Trust.Add(trust);
            }
            return package;
        }

    }
}
