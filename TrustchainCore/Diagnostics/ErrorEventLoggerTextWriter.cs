using System;
using System.Diagnostics;
using System.IO;

namespace TrustchainCore.Diagnostics
{
    public class ErrorEventLoggerTextWriter : EventLoggerTextWriter
    {
        public ErrorEventLoggerTextWriter(TextWriter defaultOut) : base(defaultOut)
        {
        }

        public override void WriteLine(string value)
        {
            // Write event log error here!
            WriteEntry(value, EventLogEntryType.Error);

            // Write to Console 
            if (Environment.UserInteractive)
                DefaultOut.WriteLine(value);
        }




    }
}
