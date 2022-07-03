using CodeGenerator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<MainRequest> _requestCollection;
        private ILogger<MongoDBService> _logger;

        public MongoDBService(IOptions<MongoDBSettings> settings, ILogger<MongoDBService> logger)
        {
            _logger = logger;
            MongoClient client = new MongoClient(settings.Value.ConnectionString);
            var retries = 0;
            var maxRetries = 3;
            while (client.Cluster.Description.State != MongoDB.Driver.Core.Clusters.ClusterState.Connected && retries <= maxRetries)
            {
                logger.LogWarning($"[{retries}/{maxRetries}]Trying to connect to mongodb...");
                Thread.Sleep(5000);
                client = new MongoClient(settings.Value.ConnectionString);
                retries++;
            }
            if (client.Cluster.Description.State != MongoDB.Driver.Core.Clusters.ClusterState.Connected)
            {
                throw new Exception("Coudln't connect to database");
            }
            IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
            _requestCollection = database.GetCollection<MainRequest>(settings.Value.CollectionName);
        }

        public async Task<String> InsertRequestAsync(MainRequestDTO requestDTO)
        {
            var request = new MainRequest();
            request.Request = requestDTO;
            await _requestCollection.InsertOneAsync(request);
            return request.Id;
        }

        public async Task<MainRequest> GetRequestByID(string id)
        {
            return await _requestCollection.Find(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }
    }
}
