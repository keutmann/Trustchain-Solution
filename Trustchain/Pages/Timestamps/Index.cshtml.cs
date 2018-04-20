using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;
using TrustchainCore.Repository;

namespace Trustchain.Pages.Timestamps
{
    public class IndexModel : PageModel
    {
        private readonly TrustDBContext _context;

        public IndexModel(TrustDBContext context)
        {
            _context = context;
        }

        public Timestamp Timestamp { get;set; }

        public void OnGetAsync()
        {
            Timestamp = new Timestamp();
        }
    }
}
