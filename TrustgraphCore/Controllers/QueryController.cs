using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TrustchainCore.Builders;
using TrustgraphCore.Model;
using TrustgraphCore.Interfaces;
using TrustchainCore.Controllers;

namespace TrustgraphCore.Controllers
{
    [Route("api/graph/[controller]")]
    public class QueryController : ApiController
    {

        public IGraphQueryService SearchService { get; set; }
        private IQueryRequestService _queryRequestService;

        public QueryController(IGraphQueryService service, IQueryRequestService queryRequestService)
        {
            SearchService = service;
            _queryRequestService = queryRequestService;
        }

        //        public IHttpActionResult Get(string issuer, string subject, string subjectType = "", string? scope, bool? trust, bool? confirm, bool? rating)

        // GET api/
        [HttpGet]
        public ActionResult Get(string issuer, string subject, bool trust = true)
        {
            var query = new QueryRequest();
            query.Issuers = new List<Byte[]>();
            query.Issuers.Add(Convert.FromBase64String(issuer));

            query.Subjects = new List<SubjectQuery>();
            query.Subjects.Add(new SubjectQuery { Id = Convert.FromBase64String(subject), Type = "" });

            query.Claim = TrustBuilder.CreateTrustTrue().ToString();
            query.Scope = string.Empty; // Global

            _queryRequestService.Verify(query);

            return ResolvePost(query);
        }

        // Post api/
        [HttpPost]
        public ActionResult ResolvePost([FromBody]QueryRequest query)
        {
            //try
            //{
            _queryRequestService.Verify(query);

            var result = SearchService.Execute(query);

            return ApiOk(result);
            //}
            //catch (Exception ex)
            //{
            //    return new ExceptionResult(ex, this);
            //}
        }


        //// GET api/
        //[HttpGet]
        //public IHttpActionResult Get([FromUri]byte[] issuer, byte[] subject, string subjectType, string scope, bool? trust, bool? confirm, bool? rating)
        //{
        //    try
        //    {
        //        var query = new GraphQuery();
        //        query.Issuer = issuer;
        //        query.Subject = subject;
        //        query.SubjectType = subjectType;
        //        query.Scope = scope;
        //        query.Claim = new JObject();
        //        if (trust != null)
        //            query.Claim.Add(new JProperty("trust", trust));

        //        if (confirm != null)
        //            query.Claim.Add(new JProperty("confirm", confirm));

        //        if (rating != null)
        //            query.Claim.Add(new JProperty("rating", 0));

        //        var result = Service.Query(query);

        //        return Ok(JsonConvert.SerializeObject(result));
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ExceptionResult(ex, this);
        //    }
        //}

    }
}
