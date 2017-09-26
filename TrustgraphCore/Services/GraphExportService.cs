using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Services
{
    public class GraphExportService : IGraphExportService
    {
        public IGraphModelService ModelService { get; }

        public GraphExportService(IGraphModelService modelService)
        {
            ModelService = modelService;
        }

        public PackageModel GetFullGraph()
        {
            var package = new PackageModel();
            package.Trust = new List<TrustModel>();

            foreach (var address in ModelService.Graph.Issuers)
            {
                var trust = new TrustModel();
                trust.IssuerId = address.Id;
                
                var subjects = new List<SubjectModel>();
                if (address.Subjects != null)
                {
                    foreach (var edge in address.Subjects)
                    {
                        var child = new SubjectModel();
                        ModelService.InitSubjectModel(child, edge);
                        subjects.Add(child);
                    }
                }
                if(subjects.Count > 0)
                    trust.Subjects = subjects;

                package.Trust.Add(trust);
            }
            return package;
        }

    }
}
