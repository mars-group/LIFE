using System;
using System.Collections.Concurrent;
using LIFE.API.Agent;
using LIFE.API.GridCommon;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.Environments.GridEnvironment;
using NUnit.Framework;

namespace BasicAgentsTests
{
    [TestFixture]
    public class BasicAgentInitTests
    {

        private IGridEnvironment<GridAgent<Tree>> _gridEnv;
        private ConcurrentDictionary<ILayer, ConcurrentDictionary<int, ConcurrentDictionary<ITickClient, byte>>> _tickClientsPerLayer;
        [SetUp]
        public void Setup()
        {
            _gridEnv = new GridEnvironment<GridAgent<Tree>>(60, 40);
            _tickClientsPerLayer = new ConcurrentDictionary<ILayer, ConcurrentDictionary<int, ConcurrentDictionary<ITickClient, byte>>>();
        }

        [Test]
        public void TestAgentCreation()
        {
            Assert.DoesNotThrow(() =>
            {
                var l = new TreeLayer(_gridEnv);
                if (!_tickClientsPerLayer.ContainsKey(l))
                {
                    _tickClientsPerLayer.TryAdd(l,new ConcurrentDictionary<int, ConcurrentDictionary<ITickClient, byte>>());
                }


                l.InitLayer(null, RegisterAgentHandle, UnregisterAgentHandle);
            });
        }

        private void UnregisterAgentHandle(ILayer layer, ITickClient tickClient, int executionInterval)
        {

        }

        private void RegisterAgentHandle(ILayer layer, ITickClient tickClient, int executionInterval)
        {
            // make sure execution group is available
            _tickClientsPerLayer[layer].GetOrAdd(executionInterval, new ConcurrentDictionary<ITickClient, byte>());
            // add tickClient to execution group
            _tickClientsPerLayer[layer][executionInterval].TryAdd(tickClient, new byte());
        }

        [Test]
        public void TestShit()
        {
            var dict = new ConcurrentDictionary<C, byte>();
            var b = new B();
            dict.TryAdd(b, new byte());
        }
    }

    interface C {}

    internal class B : A<B>, IEquatable<B>
    {
        public bool Equals(B other)
        {
            return this.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((B) obj);
        }

    }

    internal abstract class A<T> : C where T : IEquatable<T>{ }


    internal class Tree : GridAgent<Tree>
    {
        public Tree(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IGridEnvironment<GridAgent<Tree>> env,
            GridPosition initialPosition = null, byte[] id = null, int freq = 1)
            : base(layer, regFkt, unregFkt, env, initialPosition, id, freq)
        {

        }

        protected override IInteraction Reason()
        {
            return null;
        }

        public override Tree AgentReference => this;
    }

    internal class TreeLayer : ISteppedLayer
    {
        private long _currentTick;
        private IGridEnvironment<GridAgent<Tree>> _env;

        public TreeLayer(IGridEnvironment<GridAgent<Tree>> env)
        {
            _env = env;
        }

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            var t = new Tree(this, registerAgentHandle, unregisterAgentHandle, _env);
            return true;
        }

        public long GetCurrentTick()
        {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick)
        {
            _currentTick = currentTick;
        }
    }
}