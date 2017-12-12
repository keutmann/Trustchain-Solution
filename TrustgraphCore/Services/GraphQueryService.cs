using System;
using System.Linq;
using System.Collections.Generic;
using TrustgraphCore.Model;
using TrustchainCore.Extensions;
using TrustgraphCore.Interfaces;
using TrustchainCore.Interfaces;
using TrustgraphCore.Enumerations;
using TrustchainCore.Model;

namespace TrustgraphCore.Services
{
    public class GraphQueryService : IGraphQueryService
    {
        public IGraphModelService ModelService { get; }
        public long UnixTime { get; set; }
        private ITrustDBService _trustDBService;

        public GraphQueryService(IGraphModelService modelService, ITrustDBService trustDBService)
        {
            ModelService = modelService;
            _trustDBService = trustDBService;
            UnixTime = DateTime.Now.ToUnixTime();
        }

        public QueryContext Execute(QueryRequest query)
        {
            var context = new QueryContext(ModelService, query);
            
            if(context.IssuerIndex.Count > 0 && context.TargetIndex.Count > 0)
                ExecuteQueryContext(context);

            if (context.Results.Count > 0)
            {
                context.Subjects = BuildResultNode(context);//result.Node = BuildResultTree(context);
                if(_trustDBService != null)
                    AddClaims(context);
            }

            return context;
        }

        protected void AddClaims(QueryContext context)
        {
            IteratedSubjects(context.Subjects, (item) => {

                foreach (var target in context.TargetIndex)
                {
                    if (item.SubjectIndex != target.Id)
                        continue;

                    if ((item.ClaimModel.Metadata & ClaimMetadata.Reason) > 0) // A reason property exist on this subject
                    {
                        var dbItem = _trustDBService.Subjects.FirstOrDefault(w => w.Address == item.Address);
                        if (dbItem != null)
                            item.ClaimIndexs = dbItem.ClaimIndexs;
                    }
                }
            });
        }

        protected void IteratedSubjects(IList<SubjectResult> subjects, Action<SubjectResult> callback)
        {
            foreach (var item in subjects)
            {
                if(item.Subjects != null && item.Subjects.Count > 0)
                    IteratedSubjects(item.Subjects, callback);
                else
                    callback(item);
            }
        }


        protected List<SubjectResult> BuildResultNode(QueryContext context)
        {
            var results = new List<SubjectResult>();
            var nodelist = new Dictionary<Int64Container, SubjectResult>();
            var subjectNodes = new List<SubjectResult>();
            foreach (var result in context.Results)
            {
                var subject = new SubjectResult();
                subject.SubjectIndex = result.Subject.SubjectId;
                subject.ParentIndex = result.IssuerIndex;
                var visited = context.Visited[result.IssuerIndex];

                var claim = new Claim();

                subject.GraphSubjectIndex = new Int64Container(result.IssuerIndex, visited.SubjectIndex);
                ModelService.InitSubjectModel(subject, claim, result.Subject);
                subject.NametIndex = result.Subject.NameIndex;
                subject.ClaimModel = result.Subject.Claim;

                subjectNodes.Add(subject);
            }

            while (results.Count == 0)
            {
                var currentLevelNodes = new List<SubjectResult>();
                foreach (var subject in subjectNodes)
                {
                    var parentNode = new SubjectResult();
                    parentNode.SubjectIndex = subject.ParentIndex;
                    parentNode.Name = ModelService.Graph.NameIndexReverse[subject.NametIndex]; // Ensure that the name of the issuer is returned
                    //subject.Name = 
                    var visited = context.Visited[subject.SubjectIndex];
                    parentNode.ParentIndex = context.Visited[subject.ParentIndex].ParentIndex;
                    parentNode.GraphSubjectIndex = new Int64Container(parentNode.SubjectIndex, visited.SubjectIndex);

                    if (nodelist.ContainsKey(parentNode.GraphSubjectIndex))
                    {
                        // A previouse node in the collection has already created this
                        nodelist[parentNode.GraphSubjectIndex].Subjects.Add(subject);
                        continue;
                    }

                    var issuer = ModelService.Graph.Issuers[parentNode.SubjectIndex];
                    parentNode.Address = issuer.Id;

                    if (visited.SubjectIndex >= 0)
                    {
                        var claim = new Claim();
                        var edge = issuer.Subjects[visited.SubjectIndex];
                        ModelService.InitSubjectModel(parentNode, claim, edge);
                    }

                    parentNode.Subjects = new List<SubjectResult> { subject };

                    currentLevelNodes.Add(parentNode);
                    nodelist.Add(parentNode.GraphSubjectIndex, parentNode);

                    if (context.IssuerIndex.Contains(parentNode.SubjectIndex))
                    {
                        results.Add(parentNode);
                        continue;
                    }

                }
                subjectNodes = currentLevelNodes;
            }

            return results;
        }



        protected void ExecuteQueryContext(QueryContext context)
        {
            List<QueueItem> queue = new List<QueueItem>();
            foreach (var index in context.IssuerIndex)
                queue.Add(new QueueItem(index, -1, -1, 0)); // Starting point!

            while (queue.Count > 0 && context.Level < 4) // Do go more than 4 levels down
            {
                context.TotalIssuerCount += queue.Count;

                // Check current level for trust
                foreach (var item in queue)
                    PeekNode(item, context);

                // Stop here if trust found at current level
                if (context.Results.Count > 0)
                    break; // Stop processing the query!

                // Continue to next level
                var subQueue = new List<QueueItem>();
                foreach (var item in queue)
                    subQueue.AddRange(Enqueue(item, context));

                queue = subQueue;

                context.Level++;
            }
        }

        protected bool PeekNode(QueueItem item, QueryContext context)
        {
            int found = 0;
            context.SetVisitItemSafely(item.Index, new VisitItem(item.ParentIndex, item.EdgeIndex)); // Makes sure that we do not run this block again.
            var subjects = ModelService.Graph.Issuers[item.Index].Subjects;
            if (subjects == null)
                return false;

            for (var i = 0; i < subjects.Length; i++)
            {
                context.TotalSubjectCount++;

                if (subjects[i].Activate > UnixTime ||
                   (subjects[i].Expire > 0 && subjects[i].Expire < UnixTime))
                    continue;

                if ((subjects[i].Claim.Types & context.Claim.Types) == 0)
                    continue; 

                //if (edges[i].SubjectType != context.Query.SubjectType ||
                if(context.Scope != 0 &&
                    subjects[i].Scope != context.Scope)
                    continue; // No claims match query

                context.MatchSubjectCount++;

                for(var t = 0; t < context.TargetIndex.Count; t++) 
                    if (context.TargetIndex[t].Id == subjects[i].SubjectId)
                    {
                        var result = new ResultNode();
                        result.IssuerIndex = item.Index;
                        result.ParentIndex = item.ParentIndex;
                        result.Subject = subjects[i];
                        context.Results.Add(result);
                        found ++;
                        if (found >= context.TargetIndex.Count) // Do not look further, because we found them all.
                            return true;
                    }
            }

            return found != 0;
        }

        protected List<QueueItem> Enqueue(QueueItem item, QueryContext context)
        {
            var list = new List<QueueItem>();
            var address = ModelService.Graph.Issuers[item.Index];

            var subjects = address.Subjects;
            if (subjects == null)
                return list;

            for (var i = 0; i < subjects.Length; i++)
            {
                //if (edges[i].SubjectType != context.Query.SubjectType ||
                if(context.Scope != 0 && 
                    subjects[i].Scope != context.Scope)
                    continue; // Do not follow when Trust do not match scope or SubjectType

                if (subjects[i].Activate > UnixTime ||
                    (subjects[i].Expire > 0 && subjects[i].Expire < UnixTime)) 
                    continue; // Do not follow when Trust has not activated or has expired

                if ((subjects[i].Claim.Flags & ClaimType.Trust) == 0)
                    continue; // Do not follow when trust is false or do not exist.

                var visited = context.GetVisitItemSafely(subjects[i].SubjectId);
                if(visited.ParentIndex > -1) // If parentIndex is -1 then it has not been used yet!
                {
                    var parentAddress = ModelService.Graph.Issuers[visited.ParentIndex];
                    var visitedEdge = parentAddress.Subjects[visited.SubjectIndex];
                    if (visitedEdge.Cost > subjects[i].Cost) // If the current cost is lower then its a better route.
                        context.Visited[subjects[i].SubjectId] = new VisitItem(item.Index, i); // Overwrite the old visit with the new because of lower cost

                    continue; // We have already done this node, so no need to reprocess.
                }

                list.Add(new QueueItem(subjects[i].SubjectId, item.Index, i, subjects[i].Cost));
            }
            return list;
        }
    }
}

