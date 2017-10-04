using System;
using System.Collections.Generic;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;

namespace TrustchainCore.Services
{
    public interface IWorkflowService
    {
        ITrustDBService DBService { get; set; }
        IServiceProvider ServiceProvider { get; set; }

        T Create<T>(WorkflowEntity workflow) where T : IWorkflowContext, new();
        void Execute(IList<IWorkflowContext> workflows);
        void Save(IWorkflowContext context);
    }
}