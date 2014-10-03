using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using ForestLayer.Agents;
using ForestLayer.TransportTypes;
using Hik.Collections;
using LayerAPI.Interfaces;
using mars.rock.drill;
using Mono.Addins;
using System.Linq;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace AWRShowcase
{
    [Extension(typeof(ISteppedLayer))]
    public class ForestLayer : ISteppedLayer
    {
        private const int ScalingFactor = 100;
        private readonly ThreadSafeSortedList<Guid, Tree> _treeList;
        private readonly Random _rand;

        public ForestLayer()
        {
            _treeList = new ThreadSafeSortedList<Guid, Tree>();
            _rand = new Random();
        }

		/// <summary>
		/// Inits the forest layer from data placed in the MARS ROCK db.
		/// </summary>
		/// <returns><c>true</c>, if layer was inited, <c>false</c> otherwise.</returns>
		/// <param name="layerInitData">Layer init data.</param>
		/// <param name="registerAgentHandle">Register agent handle.</param>
		/// <param name="unregisterAgentHandle">Unregister agent handle.</param>
        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            Drill.InitializeConnection("rock.mars.haw-hamburg.de", "mars", "rock");
            var cubeAwr = Drill.Cubes.FirstOrDefault(cube => cube.CubeName == "awr");
            if (cubeAwr == null) throw new IOException("Could not load data from database.");
            DataTable resultTable = cubeAwr.GetData();
            Parallel.For(0, resultTable.Rows.Count, delegate(int i)
            {
                var row = resultTable.Rows[i];
                var biomass = double.Parse(row["t_biomass"].ToString());
                var height = double.Parse(row["t_height"].ToString());
                var diameter = double.Parse(row["t_diameter"].ToString());
                var crownDiameter = double.Parse(row["t_crown"].ToString());
                var lat = double.Parse(row["s_lat"].ToString());
                var lon = double.Parse(row["s_lon"].ToString());

                Parallel.For
                    (0, ScalingFactor,
                        delegate(int j)
                        {
                            var tree = new Tree(height, diameter, crownDiameter, 5, biomass, lat, lon);
                            _treeList[tree.TreeId] = tree;
                        });
            });

            foreach (var tree in _treeList.GetAllItems())
            {
                registerAgentHandle.Invoke(this, tree);
            }
            Console.WriteLine("Initialized " + _treeList.Count + " trees!");
            return true;
        }

        public long GetCurrentTick()
        {
            throw new System.NotImplementedException();
        }

        public TTree GetTree()
        {
            return new TTree(_treeList.GetAllItems().ToArray()[_rand.Next(_treeList.Count)]);
        }

        public bool CutTree(Guid treeId)
        {

            return _treeList.Remove(treeId);
        }

    }

}
