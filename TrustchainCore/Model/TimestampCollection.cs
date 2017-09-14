using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustchainCore.Model
{
    public class TimestampCollection : Dictionary<string, TimestampModel>
    {
        public TimestampCollection() : base(StringComparer.InvariantCultureIgnoreCase)
        {

        }
    }
}
