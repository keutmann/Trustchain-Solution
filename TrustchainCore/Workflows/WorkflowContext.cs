using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrustchainCore.Enumerations;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;

namespace TrustchainCore.Workflows
{
    public class WorkflowContext : IWorkflowContext
    {
        [JsonIgnore]
        public int ID { get; set; }

        [JsonIgnore]
        public string State { get; set; }

        [JsonIgnore]
        public string Tag { get; set; }


        public int CurrentStepIndex { get; set; }

        [JsonProperty(PropertyName = "steps", NullValueHandling = NullValueHandling.Ignore)]
        public IList<IWorkflowStep> Steps { get; set; }

        [JsonIgnore]
        public WorkflowStatusType Status { get; set; }

        [JsonProperty(PropertyName = "logs", NullValueHandling = NullValueHandling.Ignore)]
        public List<IWorkflowLog> Logs { get; set; }

        protected IWorkflowService _workflowService;

        public WorkflowContext(IWorkflowService workflowService) 
        {
            _workflowService = workflowService;
        }

        public virtual void Initialize()
        {
            Steps = new List<IWorkflowStep>();
            Logs = new List<IWorkflowLog>();
        }

        public virtual async Task Execute()
        {
            await Task.Run(() =>
            {
                while (TryGetNextStep(out IWorkflowStep step))
                {
                    try
                    {
                        ExecutedStep(step);

                        CurrentStepIndex++;
                    }
                    catch (Exception ex)
                    {
                        Failed(step, ex);
                        return; // Exit fast!
                    }
                    finally
                    {
                        step.Dispose();
                        Save();
                    }
                }
            });

            Success();
        }

        public T GetStep<T>()
        {
            foreach (var step in Steps)
            {
                if (typeof(T).IsAssignableFrom(step.GetType()))
                    return (T)step;
            }

            return default(T);
        }

        public virtual void Save()
        {
            _workflowService.Save(this);
        }

        public virtual bool TryGetNextStep(out IWorkflowStep step)
        {
            step = null;
            if (Steps.Count == 0 || CurrentStepIndex >= Steps.Count)
                return false;

            step = Steps[CurrentStepIndex];

            return true;
        }

        private void ExecutedStep(IWorkflowStep step)
        {
            step.Execute();
            Log($"{step.GetType().Name} has executed.");
        }

        public virtual void Success()
        {
            Status = WorkflowStatusType.Finished;
            Log("Workflow completed successfully");
        }

        public virtual void Failed(IWorkflowStep step, Exception ex)
        {
            Status = WorkflowStatusType.Failed;
            Log($"Step: {step.GetType().Name} has failed with an error: {ex.Message}");
        }

        public virtual void Log(string message)
        {
            Logs.Add(new WorkflowLog { Message = message });
        }
    }
}
