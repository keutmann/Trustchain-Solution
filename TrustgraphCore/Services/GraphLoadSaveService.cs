﻿using System.Linq;
using Microsoft.Extensions.Logging;
using TrustchainCore.Interfaces;
using TrustgraphCore.Interfaces;
using System;
using TrustchainCore.Extensions;

namespace TrustgraphCore.Services
{
    public class GraphLoadSaveService : IGraphLoadSaveService
    {
        private IGraphTrustService _graphTrustService;

        private ITrustDBService _trustDBService;
        private ILogger _logger;

        public GraphLoadSaveService(IGraphTrustService graphTrustService, ITrustDBService trustDBService, ILoggerFactory loggerFactory)
        {
            _graphTrustService = graphTrustService;
            _trustDBService = trustDBService;
            _logger = loggerFactory.CreateLogger<GraphLoadSaveService>();
        }

        public void LoadFromDatabase()
        {
            _logger.LogInformation("Loading trust into Graph");
            var count = 0;
            // No need to load packages, just load trusts directly.
            var time = DateTime.Now.ToUnixTime();

            var trusts = from trust in _trustDBService.Trusts
                     where (trust.Activate <= time || trust.Activate == 0) && (trust.Expire > time || trust.Expire == 0)
                     select trust;

            foreach (var trust in trusts)
            {
                count++;
                _graphTrustService.Add(trust);
            }
            _logger.LogInformation($"Trust loaded: {count}");
        }


        //public void LoadGraphSnapshot(string filename)
        //{
        //    _graphTrustService.Graph = JsonConvert.DeserializeObject<GraphModel>(File.ReadAllText(filename));
        //}

        //public void SaveGraphSnapshot(string filename)
        //{
        //    File.WriteAllText(filename, JsonConvert.SerializeObject(_graphTrustService.Graph, Formatting.Indented));
        //}

    }
}
