using System;

namespace TrustchainCore.Workflow
{
    public class SleepWorkflow : WorkflowBase
    {

        protected DateTime DateTimeOfInstance;
        public DateTime TimeoutDate;
        public Type NextWorkflow;


        public override bool Initialize()
        {
            DateTimeOfInstance = DateTime.Now;

            TimeoutDate = (DateTime)Context.KeyValue["SleepWorkflow_TimeoutDate"];
            NextWorkflow = (Type)Context.KeyValue["SleepWorkflow_NextWorkflow"];

            return base.Initialize();
        }

        public static void Enqueue(WorkflowContext context, DateTime timeoutDate, Type nextWorkflow) 
        {
            context.KeyValue["SleepWorkflow_TimeoutDate"] = timeoutDate;
            context.KeyValue["SleepWorkflow_NextWorkflow"] = nextWorkflow;
            context.Enqueue(typeof(SleepWorkflow));
        }

        public override void Execute()
        {
            if (DateTimeOfInstance < TimeoutDate)
                return; // Not ready yet!

            Context.Enqueue(NextWorkflow);
        }
    }
}
