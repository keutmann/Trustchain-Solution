using TruststampCore.Interfaces;

namespace TruststampCore.Services
{
    public class TimestampSynchronizationService : ITimestampSynchronizationService
    {

        public int CurrentWorkflowID
        {
            get;
            set;
        }

        public TimestampSynchronizationService()
        {
            CurrentWorkflowID = 0;
        }
    }
}
