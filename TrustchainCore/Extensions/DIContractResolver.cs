using Newtonsoft.Json.Serialization;
using System;
using TrustchainCore.Interfaces;

namespace TrustchainCore.Extensions
{
    /// <summary>
    /// http://www.dotnet-programming.com/post/2017/05/08/Aspnet-core-Deserializing-Json-with-Dependency-Injection.aspx
    /// </summary>
    public class DIContractResolver : CamelCasePropertyNamesContractResolver
    {
        IDIMeta diMeta;
        IServiceProvider sp;
        public DIContractResolver(IDIMeta diMeta, IServiceProvider sp)
        {
            this.diMeta = diMeta;
            this.sp = sp;
        }
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {

            if (diMeta.IsRegistred(objectType))
            {
                JsonObjectContract contract = DIResolveContract(objectType);
                contract.DefaultCreator = () => sp.GetService(objectType);

                return contract;
            }

            return base.CreateObjectContract(objectType);
        }
        private JsonObjectContract DIResolveContract(Type objectType)
        {
            var fType = diMeta.RegistredTypeFor(objectType);
            if (fType != null) return base.CreateObjectContract(fType);
            else return CreateObjectContract(objectType);
        }
    }
}
