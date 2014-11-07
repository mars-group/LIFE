using LayerAPI.Interfaces;

namespace VisualizationAdapter.Interface
{
    interface IVisualizationAdapterInternal : IVisualizationAdapterPublic {
        void RegisterVisualizable(IVisualizable visualizable);
        void VisualizeTick();
    }
}
