using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using System.Collections;

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

        public async Task<IActionResult> OnGetAsync(byte[] source)
        {
            if (source == null)
            {
                return NotFound();
            }

            Timestamp = await _context.Timestamps.SingleOrDefaultAsync(m => StructuralComparisons.StructuralEqualityComparer.Equals(m.Source, source));

            if (Timestamp == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
