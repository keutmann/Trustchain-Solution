using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;
using TrustchainCore.Repository;

namespace Trustchain.Pages.Proofs
{
    public class DetailsModel : PageModel
    {
        private readonly TrustchainCore.Repository.TrustDBContext _context;

        public DetailsModel(TrustchainCore.Repository.TrustDBContext context)
        {
            _context = context;
        }

        public ProofEntity ProofEntity { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProofEntity = await _context.Proofs.SingleOrDefaultAsync(m => m.ID == id);

            if (ProofEntity == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
