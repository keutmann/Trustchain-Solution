using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Trustchain.Extensions;
using TrustchainCore.Repository;

namespace Trustchain.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TrustDBContext _context;
        
        public IndexModel(TrustDBContext context)
        {
            _context = context;
        }

        public void OnGet(string command)
        {
            // Temp hack for controlling data on the database
            //if(command == "cleandatabase")
            //{
            //    _context.ClearAllData();
            //}
        }
    }
}
