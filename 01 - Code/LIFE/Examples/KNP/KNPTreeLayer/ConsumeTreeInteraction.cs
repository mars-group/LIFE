using DalskiAgent.Reasoning;
using TreeLayer.Agents;

namespace TreeLayer
{
    class ConsumeTreeInteraction : IInteraction
    {
        private readonly ITree _tree;
        private readonly ITree _otherTree;


        public ConsumeTreeInteraction(ITree tree, ITree otherTree) {
            _tree = tree;
            _otherTree = otherTree;
        }

        public void Execute() {
           _tree.Biomass += _otherTree.Biomass;
        }
    }
}
