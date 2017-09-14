using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustchainCore.Workflow
{
    public class WorkflowEngine
    {
        public List<WorkflowContext> Tasks = new List<WorkflowContext>();

        public virtual void Execute()
        {
            List<Task> executing = new List<Task>();
            foreach (var task in Tasks)
            {
                executing.Add(task.Execute());
            }

            Task.WaitAll(executing.ToArray());
        }
    }
}
