using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using TrustchainCore.Collections.Generic;
using TrustchainCore.Extensions;
using System.Collections;
using Trustchain.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;

namespace Trustchain.Pages.Timestamps
{
    public class IndexModel : ListPageModel<Timestamp>
    {
        private readonly TrustDBContext _context;

        public IndexModel(TrustDBContext context)
        {
            _context = context;
        }


        public async Task OnGetAsync(string sortOrder, string sortField, string currentFilter, string searchString, byte[] source, int? pageIndex)
        {
            InitProperties(sortOrder, sortField, currentFilter, searchString, pageIndex);

            var query = from s in _context.Timestamps
                        select s;

            if (source != null)
                query = query.Where(p => StructuralComparisons.StructuralEqualityComparer.Equals(p.Source, source));
            else
                query = BuildQuery(CurrentFilter, query);

            query = AddSorting(query, "Registered", "_desc");

            List = await PaginatedList<Timestamp>.CreateAsync(query.AsNoTracking(), PageIndex, PageSize);
        }

        public RouteValueDictionary GetParam(Timestamp timestamp)
        {
            var dict = InitParam();

            dict["source"] = Convert.ToBase64String(timestamp.Source);

            return dict;
        }

        private IQueryable<Timestamp> BuildQuery(string searchString, IQueryable<Timestamp> query)
        {
            if (String.IsNullOrEmpty(searchString))
                return query;

            if (searchString.IsHex())
            {
                var hex = searchString.FromHexToBytes();
                query = query.Where(s => StructuralComparisons.StructuralEqualityComparer.Equals(s.Source, hex));
                query = query.Where(s => StructuralComparisons.StructuralEqualityComparer.Equals(s.Receipt, hex));

                return query;
            }

            if (DateTime.TryParse(searchString, out DateTime time))
            {
                var unixTime = time.ToUnixTime();
                query = query.Where(s => s.Registered == unixTime);
                return query;
            }

            if (int.TryParse(searchString, out int workflowId))
                query = query.Where(s => s.WorkflowID == workflowId);

            var likeSearch = $"%{searchString}%";
            query = query.Where(s => EF.Functions.Like(s.Blockchain, likeSearch)
                || EF.Functions.Like(s.Algorithm, likeSearch)
                || EF.Functions.Like(s.Service, likeSearch));

            return query;
        }

    }
}
