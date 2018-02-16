using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TrustchainCore.Builders;
using TrustgraphCore.Model;
using TrustgraphCore.Interfaces;
using TrustchainCore.Controllers;
using TrustgraphCore.Builders;
using TrustchainCore.Model;
using TrustgraphCore.Enumerations;

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
        //[HttpGet]
        //public ActionResult Get(string issuer, string subject, bool trust = true)
        //{
        //    var builder = new QueryRequestBuilder("", TrustBuilder.BINARYTRUST_TC1);
        //    var sub = new Subject
        //    {
        //        Address = Convert.FromBase64String(subject),
        //        Type = ""
        //    };
        //    builder.Add(Convert.FromBase64String(issuer), sub);

        //    _queryRequestService.Verify(builder.Query);

        //    return ResolvePost(builder.Query);
        //}

        [HttpGet]
        public ActionResult Get(byte[] issuer, byte[] subject, QueryFlags flags = QueryFlags.LeafsOnly)
        {
            var builder = new QueryRequestBuilder(TrustScope.Global, TrustBuilder.BINARYTRUST_TC1);
            builder.Query.Flags = flags;
            builder.Add(issuer, subject, "");

            _queryRequestService.Verify(builder.Query);

            return ResolvePost(builder.Query);
        }

        // Post api/
        [HttpPost]
        public ActionResult ResolvePost([FromBody]QueryRequest query)
        {
            _queryRequestService.Verify(query);

            var result = SearchService.Execute(query);

            return ApiOk(result);
        }
    }
}
