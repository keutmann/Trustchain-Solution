namespace TrustchainCore.Workflow
{
    public class SuccessWorkflow : WorkflowBase
    {
        public override void Execute()
        {
            Context.Status = WorkflowStatus.Finished;
            Context.Log("Workflow completed successfully");
        }
    }
}
