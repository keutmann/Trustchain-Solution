﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace TrustchainCore.Interfaces
{
    public interface IWorkflowStep : IDisposable
    {
        [JsonIgnore]
        IWorkflowContext Context { get; set; }

        void Execute();
        void CombineLog(ILogger logger, string msg);
    }
}