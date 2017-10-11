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

        T Create<T>(WorkflowEntity workflow = null) where T : IWorkflowContext;
        void Execute(IList<IWorkflowContext> workflows);
        int Save(IWorkflowContext context);
        WorkflowEntity CreateWorkflowEntity(IWorkflowContext context);
    }
}