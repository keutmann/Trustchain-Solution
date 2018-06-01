using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrustchainCore.Extensions;

namespace TrustchainCore.Services
{
    public class WorkflowHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private IConfiguration _configuration;

        public WorkflowHostedService(IServiceProvider services, ILogger<WorkflowHostedService> logger, IConfiguration configuration)
        {
            Services = services;
            _logger = logger;
            _configuration = configuration;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Workflow Hosted Service is starting.");

            DoWork(cancellationToken);

            return Task.CompletedTask;
        }

        private void DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Workflow Hosted Service is working.");

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = Services.CreateScope())
                {
                    var workflowService = scope.ServiceProvider.GetRequiredService<IWorkflowService>();
                    {
                        workflowService.RunWorkflows();
                    }
                }
                Task.Delay(_configuration.WorkflowInterval()).Wait();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Workflow Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
