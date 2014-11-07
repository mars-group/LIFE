using LayerAPI.Interfaces;

namespace VisualizationAdapter.Interface
{
    public interface IVisualizationAdapterInternal : IVisualizationAdapterPublic {
        void RegisterVisualizable(IVisualizable visualizable);
        void VisualizeTick();
    }
}
