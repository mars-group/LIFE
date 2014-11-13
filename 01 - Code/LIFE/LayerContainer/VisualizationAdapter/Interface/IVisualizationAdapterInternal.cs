using LayerAPI.Interfaces;
using LayerAPI.Interfaces.Visualization;

namespace VisualizationAdapter.Interface
{
    public interface IVisualizationAdapterInternal : IVisualizationAdapterPublic {
        void RegisterVisualizable(IVisualizable visualizable);
        void VisualizeTick(int currentTime);
    }
}
