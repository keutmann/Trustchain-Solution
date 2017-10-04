using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Extensions
{
    /// <summary>
    /// http://www.dotnet-programming.com/post/2017/05/08/Aspnet-core-Deserializing-Json-with-Dependency-Injection.aspx
    /// </summary>
    public class JsonOptionsSetup : IConfigureOptions<MvcJsonOptions>
    {
        IServiceProvider serviceProvider;
        public JsonOptionsSetup(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public void Configure(MvcJsonOptions o)
        {
            o.SerializerSettings.ContractResolver = new DIContractResolver(serviceProvider.GetService<IDIMeta>(), serviceProvider);
        }
    }
}
