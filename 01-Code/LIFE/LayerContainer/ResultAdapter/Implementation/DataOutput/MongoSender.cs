using System;
using System.Collections.Concurrent;
using MongoDB.Driver;

namespace ResultAdapter.Implementation.DataOutput {

  internal class MongoSender {

    protected static IMongoClient _client;
    protected static IMongoDatabase _database;


    public MongoSender(string address) {
      //_client = new MongoClient(address);
      //_database = _client.GetDatabase("test");
    }


    public void SendVisualizationData(ConcurrentBag<string> json, string simId) {
    }
  }
}
