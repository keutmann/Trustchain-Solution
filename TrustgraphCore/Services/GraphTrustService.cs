using System;
using System.Linq;
using System.Collections.Generic;
using TrustchainCore.Model;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Services
{
    public class GraphTrustService : IGraphTrustService
    {
        public IGraphModelService ModelService { get; }

        public GraphTrustService(IGraphModelService modelService)
        {
            ModelService = modelService;
        }

        public void Add(PackageModel package)
        {
            Add(package.Trust);
        }

        public void Add(IEnumerable<TrustModel> trusts)
        {
            long unixTime = DateTime.Now.ToUnixTime();
            foreach (var trust in trusts)
            {
                Add(trust, unixTime);
            }
        }

        public void Add(TrustModel trust, long unixTime = 0)
        {
            if (unixTime == 0)
                unixTime = DateTime.Now.ToUnixTime();

            var index = ModelService.EnsureId(trust.IssuerId);
            var issuer = ModelService.Graph.Issuers[index]; // Remember its a copy of Issuer!
            var nameIndex = ModelService.EnsureName(trust.Name);
            var subjects = new List<GraphSubject>(issuer.Subjects ?? (new GraphSubject[0]));

            foreach (var subject in trust.Subjects)
            {
                BuildSubject(trust, subjects, subject, nameIndex);
            }

            // Remove old subjects thats expired!
            subjects.RemoveAll(e => e.Expire > 0 && e.Expire < unixTime);

            issuer.Subjects = subjects.ToArray();
            ModelService.Graph.Issuers[index] = issuer;

        }

        private void BuildSubject(TrustModel trust, List<GraphSubject> graphSubjects, SubjectModel subjectModel, int nameIndex = 0)
        {
            var graphSubject = ModelService.CreateGraphSubject(subjectModel, nameIndex, (int)trust.Timestamp2);
            var ids = new List<int>();
            // Find all edges that matchs
            for (var i = 0 ; i < graphSubjects.Count; i++)
            {
                if (graphSubjects[i].SubjectId != graphSubject.SubjectId)
                    continue;

                if (graphSubjects[i].SubjectType != graphSubject.SubjectType)
                    continue;

                if (graphSubjects[i].Scope != graphSubject.Scope)
                    continue;

                if ((graphSubjects[i].Claim.Types & graphSubject.Claim.Types) == 0)
                    continue;

                // Subject to be updated!
                ids.Add(i);
            }

            var flagTypes = graphSubject.Claim.Types.GetFlags();
            foreach (ClaimType flagtype in flagTypes)
            {
                var i = -1;
                if (ids.Count > 0)
                {
                    i = ids.FirstOrDefault(p => (graphSubjects[p].Claim.Types & flagtype) > 0);
                    if (graphSubjects[i].Timestamp > graphSubject.Timestamp) // Make sure that we cannot overwrite with old data
                        continue;
                }

                var nodeEdge = graphSubject; // Copy the subjectEdge object
                nodeEdge.Claim.Types = flagtype; // overwrite the flags
                nodeEdge.Claim.Flags = graphSubject.Claim.Flags & flagtype; // overwrite the flags
                nodeEdge.Claim.Rating = (flagtype == ClaimType.Rating) ? graphSubject.Claim.Rating : (byte)0;

                if (i >= 0 && i < graphSubjects.Count)
                    graphSubjects[i] = nodeEdge;
                else
                    graphSubjects.Add(nodeEdge);
            }
        }
    }
}
