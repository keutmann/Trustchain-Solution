using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrustchainCore.Enumerations;
using TrustchainCore.Interfaces;
using TrustchainCore.Services;
using TrustchainCore.Extensions;
using Microsoft.Extensions.Logging;

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

        //public int CurrentStepIndex { get; set; }
        public string CurrentStep { get; set; }

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

        public virtual void Execute()
        {
            if (!DoExecution())
                return;

            UpdateState();

            IWorkflowStep step = null;
            while(DoExecution())
            {
                try
                {
                    step = GetCurrentStep();

                    ExecutedStep(step);
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
                    found = true;
                    CurrentStep = typeof(T).AssemblyQualifiedName;
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
            CurrentStep = typeof(T).AssemblyQualifiedName;

            return instance; 
        }

        public virtual bool DoExecution()
        {
            if (Steps.Count == 0)
                return false;

            if (NextExecution > DateTime.Now.ToUnixTime())
                return false;

            if (State == WorkflowStatusType.Finished.ToString() || State == WorkflowStatusType.Failed.ToString())
                return false;
            
            return true;
        }

        public virtual void UpdateState()
        {
            if (WorkflowStatusType.New.ToString().EqualsIgnoreCase(State) || WorkflowStatusType.Starting.ToString().EqualsIgnoreCase(State))
                State = WorkflowStatusType.Running.ToString();
        }

        public virtual void Save()
        {
            WorkflowService.Save(this);
        }

        public IWorkflowStep GetCurrentStep()
        {
            if (String.IsNullOrEmpty(CurrentStep))
                return Steps[0];

            var currentType = Type.GetType(CurrentStep);
            var step = Steps.FirstOrDefault(p => currentType.IsAssignableFrom(p.GetType()));
            return step;
        }

        //public virtual bool TryGetNextStep(out IWorkflowStep step)
        //{
        //    step = null;
        //    if (Steps.Count == 0 || CurrentStepIndex >= Steps.Count)
        //        return false;

        //    step = Steps[CurrentStepIndex];

        //    return true;
        //}

        public virtual void Wait(int seconds)
        {
            NextExecution = DateTime.Now.AddSeconds(seconds).ToUnixTime();
        }

        private void ExecutedStep(IWorkflowStep step)
        {
            step.Execute();
            Log($"{step.GetType().Name} has executed.");
        }

        //public virtual void Success()
        //{
        //    State = WorkflowStatusType.Finished.ToString();
        //    Log("Workflow completed successfully");
        //}

        public virtual void Failed(IWorkflowStep step, Exception ex)
        {
            State = WorkflowStatusType.Failed.ToString();

#if DEBUG
            Log($"Step: {step.GetType().Name} has failed with an error: {ex.Message} - {ex.StackTrace}");
#else
            Log($"Step: {step.GetType().Name} has failed with an error: {ex.Message}");
#endif            

        }

        public virtual void Log(string message)
        {
            Logs.Add(new WorkflowLog { Message = message });

            if(Logs.Count > 200)
            {

            }
        }
    }
}
