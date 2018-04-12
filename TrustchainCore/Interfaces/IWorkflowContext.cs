﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrustchainCore.Model;
using TrustchainCore.Services;

namespace TrustchainCore.Interfaces
{
    public interface IWorkflowContext 
    {
        [JsonIgnore]
        IWorkflowService WorkflowService { get; set; }

        [JsonIgnore]
        WorkflowContainer Container { get; set; }

        int ID { get; set; }
        List<IWorkflowLog> Logs { get; set; }
        IList<IWorkflowStep> Steps { get; set; }
        //int CurrentStepIndex { get; set; }
        string CurrentStep { get; set; }
        long Created { get; set; }

        bool DoExecution();
        void Wait(int seconds);

        void Initialize();
        void Execute();
        T GetStep<T>();
        T AddStep<T>() where T : IWorkflowStep;
        void RunStep<T>(int seconds = 0) where T : IWorkflowStep;
        void Log(string message);
        
    }
}