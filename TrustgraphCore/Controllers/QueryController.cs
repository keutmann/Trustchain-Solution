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
using TrustchainCore.Interfaces;
using System.Text;
using TrustchainCore.Strategy;

namespace TrustgraphCore.Controllers
{
    [Route("api/graph/[controller]")]
    public class QueryController : ApiController
    {

        public IGraphQueryService SearchService { get; set; }
        private IQueryRequestService _queryRequestService;
        public IServiceProvider ServiceProvider { get; set; }


        public QueryController(IGraphQueryService service, IQueryRequestService queryRequestService, IServiceProvider serviceProvider)
        {
            SearchService = service;
            _queryRequestService = queryRequestService;
            ServiceProvider = serviceProvider;
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
            builder.Add(issuer, subject);

            _queryRequestService.Verify(builder.Query);

            return ResolvePost(builder.Query);
        }

        // Post api/
        [HttpPost]
        public ActionResult ResolvePost([FromBody]QueryRequest query)
        {

#if DEBUG
            //_queryRequestService.Verify(query);
            var _trustBuilder = BuildTestGraph();

            //var result = SearchService.Execute(query);

            return ApiOk(_trustBuilder.Package);


#else
            _queryRequestService.Verify(query);

            var result = SearchService.Execute(query);

            return ApiOk(result);
#endif
        }

        protected TrustBuilder BuildTestGraph()
        {
            TrustBuilder _trustBuilder = new TrustBuilder(ServiceProvider);
            _trustBuilder.SetServer("testserver");

            _trustBuilder.AddTrust("trustchain", "CryptoQRCodeBot", TrustBuilder.CreateTrustClaim());
            //_trustBuilder.AddTrust("B", "C", TrustBuilder.CreateTrustClaim());
            //_trustBuilder.AddTrust("C", "D", TrustBuilder.CreateTrustClaim());
            //_trustBuilder.AddTrust("B", "E", TrustBuilder.CreateTrustClaim());
            //_trustBuilder.AddTrust("E", "D", TrustBuilder.CreateTrustClaim());
            //_trustBuilder.AddTrust("B", "F", TrustBuilder.CreateTrustClaim());
            //_trustBuilder.AddTrust("F", "G", TrustBuilder.CreateTrustClaim());
            //_trustBuilder.AddTrust("G", "D", TrustBuilder.CreateTrustClaim()); // Long way, no trust
            //_trustBuilder.AddTrust("G", "Unreach", TrustBuilder.CreateTrustClaim()); // Long way, no trust

            //_trustBuilder.AddTrust("A", "B", TrustBuilder.CreateConfirmClaim());
            //_trustBuilder.AddTrust("C", "D", TrustBuilder.CreateConfirmClaim());
            //_trustBuilder.AddTrust("G", "D", TrustBuilder.CreateConfirmClaim());

            //_trustBuilder.AddTrust("A", "B", TrustBuilder.CreateRatingClaim());
            //_trustBuilder.AddTrust("C", "D", TrustBuilder.CreateRatingClaim());
            //_trustBuilder.AddTrust("G", "D", TrustBuilder.CreateRatingClaim());

            //_trustBuilder.AddTrust("A", "NoTrustB", TrustBuilder.CreateTrustClaim("", false));
            //_trustBuilder.AddTrust("B", "NoTrustC", TrustBuilder.CreateTrustClaim("", false));
            //_trustBuilder.AddTrust("C", "NoTrustD", TrustBuilder.CreateTrustClaim("", false));

            //_trustBuilder.AddTrust("C", "MixD", TrustBuilder.CreateTrustClaim("", true));
            //_trustBuilder.AddTrust("E", "MixD", TrustBuilder.CreateTrustClaim("", false));

            _trustBuilder.Build().Sign();
            return _trustBuilder;
        }
    }

    internal static class TrustBuilderExtensions2
    {
        public static IDerivationStrategy ScriptService = new DerivationBTCPKH();


        public static byte[] GetAddress(string name)
        {
            var issuerKey = ScriptService.GetKey(Encoding.UTF8.GetBytes(name));
            var address = ScriptService.GetAddress(issuerKey);

            return address;
        }

        public static TrustBuilder AddTrust(this TrustBuilder builder, string name)
        {
            var issuerKey = ScriptService.GetKey(Encoding.UTF8.GetBytes(name));
            var address = ScriptService.GetAddress(issuerKey);

            builder.AddTrust().SetIssuer(address, ScriptService.ScriptName, (Identity identity, byte[] data) =>
            {
                return ScriptService.Sign(issuerKey, data);
            });

            return builder;
        }

        public static TrustBuilder AddTrust(this TrustBuilder builder, string issuerName, string subjectName, Claim trustClaim)
        {
            builder.AddTrust(issuerName).AddSubject(subjectName, trustClaim);
            return builder;
        }

        public static TrustBuilder AddTrustTrue(this TrustBuilder builder, string issuerName, string subjectName)
        {
            builder.AddTrust(issuerName, subjectName, TrustBuilder.CreateTrustClaim());
            return builder;
        }

        public static TrustBuilder AddSubject(this TrustBuilder builder, string subjectName, Claim trustClaim)
        {
            var key = ScriptService.GetKey(Encoding.UTF8.GetBytes(subjectName));
            var address = ScriptService.GetAddress(key);

            int[] indexs = new int[] { 0 };

            builder.AddClaim(trustClaim);

            builder.AddSubject(address, subjectName, new int[] { trustClaim.Index });
            return builder;
        }

        public static TrustBuilder SetServer(this TrustBuilder builder, string name)
        {
            var key = ScriptService.GetKey(Encoding.UTF8.GetBytes(name));
            var address = ScriptService.GetAddress(key);

            builder.SetServer(address, ScriptService.ScriptName, (Identity identity, byte[] data) =>
            {
                return ScriptService.Sign(key, data);
            });

            return builder;
        }

        //public static TrustBuilder AddClaim(this TrustBuilder builder, JObject data, out Claim claim)
        //{
        //    claim = new Claim
        //    {
        //        Data = data.ToString(Formatting.None)
        //    };

        //    builder.AddClaim(claim);

        //    return builder;
        //}

    }
}
