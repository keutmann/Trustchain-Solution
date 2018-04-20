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
    public class DetailsModel : PageModel
    {
        private readonly TrustDBContext _context;

        public DetailsModel(TrustDBContext context)
        {
            _context = context;
        }

        public Timestamp Timestamp { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Timestamp = await _context.Timestamps.SingleOrDefaultAsync(m => m.DatabaseID == id);

            if (Timestamp == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
