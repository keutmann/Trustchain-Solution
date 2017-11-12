using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;
using TrustchainCore.Repository;

namespace Trustchain.Pages.Workflow
{
    public class DetailsModel : PageModel
    {
        private readonly TrustchainCore.Repository.TrustDBContext _context;

        public DetailsModel(TrustchainCore.Repository.TrustDBContext context)
        {
            _context = context;
        }

        public WorkflowContainer WorkflowContainer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            WorkflowContainer = await _context.Workflows.SingleOrDefaultAsync(m => m.ID == id);

            if (WorkflowContainer == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
