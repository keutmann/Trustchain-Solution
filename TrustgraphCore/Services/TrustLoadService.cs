using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TrustchainCore.Interfaces;
using TrustgraphCore.Interfaces;

namespace TrustgraphCore.Services
{
    public class TrustLoadService : ITrustLoadService
    {
        private IGraphTrustService _graphTrustService;

        private ITrustDBService _trustDBService;
        private ILogger _logger;

        public TrustLoadService(IGraphTrustService graphTrustService, ITrustDBService trustDBService, ILoggerFactory loggerFactory)
        {
            _graphTrustService = graphTrustService;
            _trustDBService = trustDBService;
            _logger = loggerFactory.CreateLogger<TrustLoadService>();
        }

        public Task LoadDatabase()
        {
            return Task.Run(() =>
            {
                _logger.LogInformation("Loading trust into Graph");
                var count = 0;
                // No need to load packages, just load trusts directly.
                foreach (var trust in _trustDBService.Trusts)
                {
                    count++;
                    _graphTrustService.Add(trust);
                }
                _logger.LogInformation($"Trust loaded: {count}");

            });
        }


        public void LoadFile(string filename)
        {
            //IEnumerable<TrustModel> trusts = null;
            //var info = new FileInfo(filename);

            //if (".json".EqualsIgnoreCase(info.Extension))
            //    trusts = LoadJson(info);
            //else
            //    if(".db".EqualsIgnoreCase(info.Extension))
            //    trusts = LoadSQLite(info);

            //_graphTrustService.Add(trusts);
        }

        //private IEnumerable<TrustModel> LoadSQLite(FileInfo info)
        //{
        //    //using (var db = TrustchainDatabase.Open(info.FullName))
        //    //{
        //    //    var trusts = db.Trust.Select();
        //    //    foreach (var trust in trusts)
        //    //    {
        //    //        trust.Issuer.Subjects = db.Subject.Select(trust.TrustId).ToArray();
        //    //    }
        //    //    return trusts;
        //    //}
        //    return null;
        //}

        //private IEnumerable<TrustModel> LoadJson(FileInfo info)
        //{
        //    //var json = File.ReadAllText(info.FullName);
        //    //var trust = TrustManager.Deserialize(json);
        //    return null;
        //}
    }
}
