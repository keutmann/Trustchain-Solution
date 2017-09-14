namespace TrustchainCore.Workflow
{
    public class FailueWorkflow : WorkflowBase
    {
        public override void Execute()
        {
            Context.Status = WorkflowStatus.Failed;
            Context.Log("Workflow failed");
        }
    }
}
