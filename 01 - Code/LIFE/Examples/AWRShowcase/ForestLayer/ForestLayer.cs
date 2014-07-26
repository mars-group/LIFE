using System;
using System.Data;
using System.Threading.Tasks;
using ForestLayer.Agents;
using ForestLayer.TransportTypes;
using Hik.Collections;
using LayerAPI.Interfaces;
using mars.rock.service.client;
using Mono.Addins;
using System.Linq;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace AWRShowcase
{
   [Extension(typeof(ISteppedLayer))]
    public class ForestLayer : ISteppedLayer {
        private const int TreeCount = 10000;
        private readonly ThreadSafeSortedList<Guid, Tree> _treeList;
        private readonly Random _rand;

        public ForestLayer() {
            _treeList = new ThreadSafeSortedList<Guid,Tree>();
            _rand = new Random();
        }

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            DataTable resultTable;
            using (var rockServiceFact = new RockServiceFactory("http://141.22.32.178:10523/RockServiceSim"))
            {
                using (var rockService = rockServiceFact.CreateRockServiceClient()) {
                    rockService.ConnectCube("awr");
                    var dimensions = rockService.GetDimensions();
                    var dimTree = dimensions.FirstOrDefault(dim => dim.Name == "tree");
                    resultTable = rockService.GetData(dimTree);
                }
            }

            Parallel.For(0, resultTable.Rows.Count, delegate(int i)
            {
                var row = resultTable.Rows[i];
                var biomass = double.Parse(row["t_biomass"].ToString());
                var height = double.Parse(row["t_height"].ToString());
                var diameter = double.Parse(row["t_diameter"].ToString());
                var crownDiameter = double.Parse(row["t_crown"].ToString());
                var lat = double.Parse(row["s_lat"].ToString());
                var lon = double.Parse(row["s_lon"].ToString());


                var tree = new Tree(height, diameter, crownDiameter, 5, biomass);
                _treeList[tree.TreeId] = tree;
            });

            foreach (var tree in _treeList.GetAllItems()) {
                registerAgentHandle.Invoke(this, tree);
            }
            Console.WriteLine("Initialized " + _treeList.Count + " trees!");
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
