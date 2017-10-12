using System;
using System.Collections.Generic;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;

namespace TrustchainCore.Services
{
    public interface IWorkflowService
    {
        T Create<T>(WorkflowContainer container = null) where T : IWorkflowContext;
        void Execute(IList<IWorkflowContext> workflows);
        int Save(IWorkflowContext workflow);
        WorkflowContainer CreateWorkflowContainer(IWorkflowContext context);
    }
}