using System;
using System.Linq;
using TrustchainCore.Interfaces;
using TrustchainCore.Model;
using TruststampCore.Interfaces;
using TrustchainCore.Extensions;
using System.Collections;

namespace TruststampCore.Services
{
    public class TimestampService : ITimestampService
    {
        private ITimestampSynchronizationService _timestampSynchronizationService;
        private ITrustDBService _trustDBService;
        private IBlockchainProofFactory _timestampProofFactory;

        public TimestampService(ITimestampSynchronizationService timestampSynchronizationService, ITrustDBService trustDBService, IBlockchainProofFactory timestampProofFactory)
        {
            _timestampSynchronizationService = timestampSynchronizationService;
            _trustDBService = trustDBService;
            _timestampProofFactory = timestampProofFactory;
        }

        public IQueryable<Timestamp> Timestamps
        {
            get
            {
                return _trustDBService.Timestamps;
            }
        }

        public Timestamp Create(byte[] source)
        {
            var timestamp = new Timestamp
            {
                WorkflowID = _timestampSynchronizationService.CurrentWorkflowID,
                Source = source,
                Registered = DateTime.Now.ToUnixTime()
            };
            return timestamp;
        }

        public Timestamp Add(byte[] source, bool save = true)
        {
            var proof = Get(source);
            if(proof == null)
            {
                proof = Create(source);
                _trustDBService.DBContext.Timestamps.Add(proof);
                if(save)
                    _trustDBService.DBContext.SaveChanges();
            }
            return proof;
        }

        public Timestamp Get(byte[] source)
        {
            var proof = _trustDBService.Timestamps.FirstOrDefault(p => StructuralComparisons.StructuralEqualityComparer.Equals(p.Source, source));
            return proof;
        }

        public BlockchainProof GetBlockchainTimestamp(byte[] source)
        {
            var entity = Get(source);
            
            return _timestampProofFactory.Create(entity);
        }

        
    }
}
