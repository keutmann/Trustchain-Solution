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

        public Package GetFullGraph()
        {
            var package = new Package();
            package.Trusts = new List<Trust>();

            foreach (var address in ModelService.Graph.Issuer)
            {
                var trust = new Trust();
                trust.Issuer = new Identity();
                trust.Issuer.Address = address.Id;
                trust.Claims = new List<Claim>();
                var subjects = new List<Subject>();

                if (address.Subjects != null)
                {
                    foreach (var edge in address.Subjects)
                    {
                        var child = new Subject();
                        var claim = new Claim();
                        ModelService.InitSubjectModel(child, claim, edge);

                        subjects.Add(child);
                        trust.Claims.Add(claim);
                    }
                }
                if(subjects.Count > 0)
                    trust.Subjects = subjects;

                package.Trusts.Add(trust);
            }
            return package;
        }

    }
}
