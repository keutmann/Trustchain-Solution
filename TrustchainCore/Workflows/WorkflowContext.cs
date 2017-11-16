using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrustchainCore.Enumerations;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TrustchainCore.Extensions;

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

        [JsonProperty(PropertyName = "nextExecution")]
        public long NextExecution { get; set; }

        [JsonProperty(PropertyName = "created")]
        public long Created { get; set; }

        public int CurrentStepIndex { get; set; }

        [JsonProperty(PropertyName = "steps", NullValueHandling = NullValueHandling.Ignore)]
        public IList<IWorkflowStep> Steps { get; set; }

        //[JsonIgnore]
        //public WorkflowStatusType Status { get; set; }

        [JsonProperty(PropertyName = "logs", NullValueHandling = NullValueHandling.Ignore)]
        public List<IWorkflowLog> Logs { get; set; }

        [JsonIgnore]
        public IWorkflowService WorkflowService { get; set; }

        public WorkflowContext(IWorkflowService workflowService) 
        {
            WorkflowService = workflowService;
            NextExecution = DateTime.MinValue.ToUnixTime();
            Steps = new List<IWorkflowStep>();
            Logs = new List<IWorkflowLog>();
            State = WorkflowStatusType.New.ToString();
        }

        public virtual void Initialize()
        {
            Created = DateTime.Now.ToUnixTime();

            foreach (var step in Steps)
            {
                step.Context = this;
            }
        }

        public virtual async Task Execute()
        {
            await Task.Run(() =>
            {
                if (!DoExecution())
                    return;

                UpdateState();

                while (TryGetNextStep(out IWorkflowStep step))
                {
                    try
                    {
                        ExecutedStep(step);

                        CurrentStepIndex++;

                        if (!DoExecution())
                            break;
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

            if (CurrentStepIndex >= Steps.Count)
            {
                Success();
                Save();
            }
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

        public void RunStep<T>(int waitSeconds = 0) where T : IWorkflowStep
        {
            var found = false;
            for(int i = 0; i< Steps.Count; i++)
            {
                var step = Steps[i];
                if (typeof(T).IsAssignableFrom(step.GetType()))
                {
                    CurrentStepIndex = i - 1;
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                AddStep<T>();
            }

            if(waitSeconds > 0 )
            {
                Wait(waitSeconds);
            }
        }


        public virtual T AddStep<T>() where T: IWorkflowStep
        {
            var instance = WorkflowService.ServiceProvider.GetRequiredService<T>();
            instance.Context = this;
            Steps.Add(instance);
            return instance; 
        }

        public virtual bool DoExecution()
        {
            if (NextExecution > DateTime.Now.ToUnixTime())
                return false;
            return true;
        }

        public virtual void UpdateState()
        {
            if (State == WorkflowStatusType.New.ToString())
                State = WorkflowStatusType.Running.ToString();
        }

        public virtual void Save()
        {
            WorkflowService.Save(this);
        }

        public virtual bool TryGetNextStep(out IWorkflowStep step)
        {
            step = null;
            if (Steps.Count == 0 || CurrentStepIndex >= Steps.Count)
                return false;

            step = Steps[CurrentStepIndex];

            return true;
        }

        public virtual void Wait(int seconds)
        {
            NextExecution = DateTime.Now.AddSeconds(seconds).ToUnixTime();
        }

        public virtual void RunStepAgain(int seconds)
        {
            Wait(seconds);
            CurrentStepIndex--;
        }

        private void ExecutedStep(IWorkflowStep step)
        {
            step.Execute();
            Log($"{step.GetType().Name} has executed.");
        }

        public virtual void Success()
        {
            State = WorkflowStatusType.Finished.ToString();
            Log("Workflow completed successfully");
        }

        public virtual void Failed(IWorkflowStep step, Exception ex)
        {
            State = WorkflowStatusType.Failed.ToString();
            Log($"Step: {step.GetType().Name} has failed with an error: {ex.Message}");
        }

        public virtual void Log(string message)
        {
            Logs.Add(new WorkflowLog { Message = message });
        }
    }
}
