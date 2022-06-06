﻿using CodeGenerator.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<MainRequest> _requestCollection;

        public MongoDBService(IOptions<MongoDBSettings> settings)
        {
            MongoClient client = new MongoClient(settings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
            _requestCollection = database.GetCollection<MainRequest>(settings.Value.CollectionName);
        }

        public async Task InsertRequestAsync(MainRequest request)
        {
            await _requestCollection.InsertOneAsync(request);
            return;
        }

        public async Task<List<MainRequest>> GetRequestByID(String id)
        {
            return await _requestCollection.Find(x => x.Id.Equals(id)).ToListAsync();
        }
    }
}