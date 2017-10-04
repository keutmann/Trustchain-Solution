using TrustchainCore.Interfaces;

namespace TrustchainCore.Workflows
{


    public abstract class WorkflowStep : IWorkflowStep
    {
        public virtual IWorkflowContext Context { get; set; }

        public virtual void Execute()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
