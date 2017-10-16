﻿using System.Collections.Generic;
using TrustchainCore.Services;
using TruststampCore.Workflows;

namespace TruststampCore.Interfaces
{
    public interface ITimestampWorkflowService
    {
        int CurrentWorkflowID { get; }
        void CreateNextWorkflow();
        void EnsureTimestampWorkflow();
    }
}