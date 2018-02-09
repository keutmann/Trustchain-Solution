using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TrustchainCore.Builders;
using TrustgraphCore.Model;
using TrustgraphCore.Interfaces;
using TrustchainCore.Controllers;
using TrustgraphCore.Builders;
using TrustchainCore.Model;

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
            var builder = new QueryRequestBuilder("", TrustBuilder.BINARYTRUST_TC1);
            var sub = new Subject
            {
                Address = Convert.FromBase64String(subject),
                Type = ""
            };
            builder.Add(Convert.FromBase64String(issuer), sub);

            _queryRequestService.Verify(builder.Query);

            return ResolvePost(builder.Query);
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
