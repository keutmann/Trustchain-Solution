using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TrustchainCore.Controllers;
using TrustchainCore.Extensions;
using TrustchainCore.Model;
using TruststampCore.Interfaces;

namespace TruststampCore.Controllers
{
    public class ProofController : Controller
    {

        private IProofService _proofService;

        public ProofController(IProofService proofService)
        {
            _proofService = proofService;
        }
        
        [HttpPost] // string blockchain, 
        [Route("api/{blockchain}/[controller]")]
        public ActionResult Add(string blockchain, [FromBody]byte[] source)
        {
            return Ok(_proofService.AddProof(source));
        }

        [HttpGet] // string blockchain, 
        [Route("api/{blockchain}/[controller]/add/{source}")]
        public ActionResult AddString(string blockchain, string source)
        {
            if (String.IsNullOrWhiteSpace(source))
                throw new ApplicationException("Source cannot be empty.");

            var data = Encoding.UTF8.GetBytes(source);
            return Ok(_proofService.AddProof(data));
        }



        // GET api/
        [HttpGet]
        [Route("api/{blockchain}/[controller]/{source}")]
        public ActionResult Get(string blockchain, byte[] source)
        {
            return Ok(_proofService.GetBlockchainProof(source));
        }


        [HttpGet]
        [Route("api/[controller]")]
        public ActionResult Load(String sort, String order, String search, Int32 limit, Int32 offset, String ExtraParam)
        {
            // Get entity fieldnames
            List<String> columnNames = typeof(ProofEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name).ToList();

            // Create a seperate list for searchable field names   
            List<String> searchFields = new List<String>(columnNames);

            // Exclude field Iso2 for filtering 
            //searchFields.Remove("ISO2");

            // Perform filtering
            IQueryable items = _proofService.Proofs.Search(search, searchFields);

            // Sort the filtered items and apply paging
            return Ok(items.Filter(columnNames, sort, order, limit, offset));
        }
        
    }
}

