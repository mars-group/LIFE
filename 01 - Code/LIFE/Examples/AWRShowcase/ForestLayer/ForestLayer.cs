using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ForestLayer.Agents;
using ForestLayer.TransportTypes;
using Hik.Collections;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace AWRShowcase
{
    public class ForestLayer : ISteppedLayer {
        private const int TreeCount = 10000;
        private readonly ThreadSafeSortedList<Guid, Tree> _treeList;
        private readonly Random _rand;

        public ForestLayer() {
            _treeList = new ThreadSafeSortedList<Guid,Tree>();
            _rand = new Random();
        }

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            for (var i = 0; i < TreeCount; i++) {
                var tree = new Tree(10, 5, 30, 10, "Aoerties");
                _treeList[tree.TreeId] = tree;
            }
            foreach (var tree in _treeList.GetAllItems()) {
                registerAgentHandle.Invoke(this, tree);
            }
            return true;
        }

        public long GetCurrentTick() {
            throw new System.NotImplementedException();
        }

        public TTree GetTree() {
            return new TTree(_treeList.GetAllItems().ToArray()[_rand.Next(_treeList.Count)]);
        }

        public bool CutTree(Guid treeId) {
            
            return _treeList.Remove(treeId);
        }

    }

}
