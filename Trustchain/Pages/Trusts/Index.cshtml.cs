﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using TrustchainCore.Interfaces;
using TrustchainCore.Extensions;
using System.Collections;
using TrustchainCore.Collections.Generic;
using System.Linq.Expressions;

namespace Trustchain.Pages.Trusts
{
    public class IndexModel : PageModel
    {
        public int PageSize = 20;

        private readonly ITrustDBService _trustDBService;

        public PaginatedList<Trust> Trusts { get; set; }

        public string CurrentFilter { get; set; }
        public string CurrentSortField { get; set; }
        public string CurrentSortOrder { get; set; }

        public IndexModel(ITrustDBService trustDBService)
        {
            _trustDBService = trustDBService;
        }


        public async Task OnGetAsync(string sortOrder, string sortField, string currentFilter, string searchString, byte[] issuerAddress, byte[] subjectAddress, string scopeValue, int? pageIndex)
        {
            if (sortOrder.EndsWithIgnoreCase("!"))
                sortOrder = sortOrder == "!" ? "_desc" : "";

            CurrentSortField = sortField;
            CurrentSortOrder = sortOrder;


            if (searchString != null)
                pageIndex = 1;
            else
                searchString = currentFilter;
            CurrentFilter = searchString;


            var query = BuildQuery(searchString);

            if (issuerAddress != null)
                query = query.Where(p => StructuralComparisons.StructuralEqualityComparer.Equals(p.Issuer.Address, issuerAddress));

            if (subjectAddress != null)
                query = query.Where(p => StructuralComparisons.StructuralEqualityComparer.Equals(p.Subject.Address, subjectAddress));

            if (scopeValue != null)
                query = query.Where(p => p.Scope.Value == scopeValue);

            switch (CurrentSortField + CurrentSortOrder)
            {
                case "Created":
                    query = query.OrderBy(s => s.Created);
                    break;
                case "Created_desc":
                    query = query.OrderByDescending(s => s.Created);
                    break;
                default:
                    query = query.OrderByDescending(s => s.Created);
                    break;
            }

            Trusts = await PaginatedList<Trust>.CreateAsync(query, pageIndex ?? 1, PageSize);
        }

        private IQueryable<Trust> BuildQuery(string searchString)
        {
            var query = from s in _trustDBService.DBContext.Trusts.AsNoTracking()
                        select s;

            if (String.IsNullOrEmpty(searchString))
                return query;

            if (searchString.IsHex() && searchString.Length > 39)
            {
                var hex = searchString.FromHexToBytes();
                if (hex.Length == 32)
                    query = query.Where(s => StructuralComparisons.StructuralEqualityComparer.Equals(s.Id, hex));

                if (hex.Length == 20)
                    query = query.Where(s => StructuralComparisons.StructuralEqualityComparer.Equals(s.Issuer.Address, hex)
                                          || StructuralComparisons.StructuralEqualityComparer.Equals(s.Subject.Address, hex));

                return query;
            }

            if (DateTime.TryParse(searchString, out DateTime time))
            {
                var unixTime = time.ToUnixTime();
                query = query.Where(s => s.Created == unixTime
                    || s.Activate == unixTime
                    || s.Expire == unixTime);

                return query;
            }

            Expression<Func<Trust, bool>> q = null;
            if (short.TryParse(searchString, out short cost))
                q = p => p.Cost == cost;

            var likeSearch = $"%{searchString}%";
            Expression<Func<Trust, bool>> search = s => EF.Functions.Like(s.Type, likeSearch)
                || EF.Functions.Like(s.Claim, likeSearch)
                || EF.Functions.Like(s.Scope.Type, likeSearch)
                || EF.Functions.Like(s.Scope.Value, likeSearch);

            q = q.Or(search);

            query = query.Where(q);

            return query;
        }


    }
}
