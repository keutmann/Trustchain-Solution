using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using TrustchainCore.Interfaces;

namespace Trustchain.Pages.Trusts
{
    public class IndexModel : PageModel
    {
        private readonly ITrustDBService _trustDBService;

        public IndexModel(ITrustDBService trustDBService)
        {
            _trustDBService = trustDBService;
        }

        public IList<Trust> Trusts { get;set; }

        public async Task OnGetAsync(byte[] issuerAddress, byte[] subjectAddress, string scopeValue)
        {
            Trusts = await _trustDBService.GetTrusts(issuerAddress, subjectAddress, scopeValue).ToListAsync();
        }
    }
}
