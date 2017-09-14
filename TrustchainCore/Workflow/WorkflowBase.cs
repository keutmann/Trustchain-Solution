using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustchainCore.Extensions;

namespace TrustchainCore.Workflow
{


    public abstract class WorkflowBase : IDisposable
    {
        public virtual WorkflowContext Context { get; set; }

        //TruststampDatabase _dataBase = null;
        //public TruststampDatabase DataBase {
        //    get
        //    {
        //        return _dataBase ?? (_dataBase = TruststampDatabase.Open());
        //    }
        //}

        //public virtual string Name
        //{
        //    get
        //    {
        //        return GetType().Name;
        //    }
        //}

        public virtual bool Initialize()
        {
            return true;
        }

        public virtual void Execute()
        {
        }


        public virtual void Dispose()
        {
        }
    }
}
