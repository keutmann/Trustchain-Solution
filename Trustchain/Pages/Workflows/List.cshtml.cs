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
    public class ListModel : PageModel
    {
        private readonly TrustDBContext _context;

        public ListModel(TrustDBContext context)
        {
            _context = context;
        }

        public IList<WorkflowContainer> WorkflowContainer { get;set; }

        public async Task OnGetAsync(string type)
        {
            var query = from p in _context.Workflows
                        where p.Type == type
                        orderby p.DatabaseID descending
                        select p;

            WorkflowContainer = await query.ToListAsync();
        }
    }
}
