using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;
using TrustchainCore.Repository;
using TrustchainCore.Enumerations;
using TrustchainCore.Extensions;
using TrustchainCore.Services;

namespace Trustchain.Pages.Workflows
{
    public class ListModel : PageModel
    {
        private readonly TrustDBContext _context;
        private readonly IWorkflowService _workflowService;


        public ListModel(TrustDBContext context, IWorkflowService workflowService)
        {
            _context = context;
            _workflowService = workflowService;
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


        public async Task OnGetRunNowAsync(int id)
        {
            var container = await _context.Workflows.FirstOrDefaultAsync(p => p.DatabaseID == id);

            var wf = _workflowService.Create(container);
            wf.Container.NextExecution = DateTime.Now.ToUnixTime();
            wf.Container.State = WorkflowStatusType.Starting.ToString();
            _workflowService.Save(wf);

            await OnGetAsync(container.Type);
        }

        public bool ShowRunNow(WorkflowContainer container)
        {
            return WorkflowStatusType.New.ToString().EqualsIgnoreCase(container.State);
        }
    }
}
