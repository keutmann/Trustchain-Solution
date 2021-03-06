using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;
using TrustchainCore.Repository;

namespace Trustchain.Pages.Workflows
{
    public class IndexModel : PageModel
    {
        private readonly TrustchainCore.Repository.TrustDBContext _context;

        public IndexModel(TrustchainCore.Repository.TrustDBContext context)
        {
            _context = context;
        }

        public IList<WorkflowContainer> WorkflowContainer { get;set; }

        public async Task OnGetAsync()
        {
            var query = _context.Workflows.GroupBy(p => p.Type).Select(p => p.OrderByDescending(t=>t.DatabaseID).First());

            WorkflowContainer = await query.ToListAsync();
        }
    }
}
