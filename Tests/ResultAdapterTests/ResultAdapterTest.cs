using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using LIFE.API.Agent;
using LIFE.API.Results;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NUnit.Framework;
using ResultAdapter.Implementation;

namespace ResultAdapterTests {

  [TestFixture]
  public class ResultAdapterTest {


    [Test]
    public void InitResultAdapter() {

      var resultAdapter = new ResultAdapterUseCase("rc-wolves", Guid.NewGuid(), true);

      resultAdapter.Register(new SimAgent());
      resultAdapter.Register(new Wolf());


      Console.WriteLine(resultAdapter.ToString(true));
      Console.WriteLine("done!");
    }
  }


  class Wolf : ITickClient {
    public void Tick() {

    }
  }


  class SimAgent : ITickClient, ISimResult {
    public void Tick() {
    }

    public AgentSimResult GetResultData() {
      return new AgentSimResult {
        AgentData = new Dictionary<string, object> {
          {"Key1" , 42 }
        },
      };
    }
  }
}
