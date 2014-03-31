
using CommonTypes.DataTypes;
using CommonTypes.TransportTypes.SimulationControl;

namespace LayerAPI.DataTypes
{
    /// <summary>
    /// This class contains the initial distribution information as well as an accessor to the data needed to 
    /// </summary>
    public class LayerInitData
    {
        public DistributionInformation DistributionInformation { get; private set; }

        public LayerInitData(DistributionInformation distributionInformation)
        {
            DistributionInformation = distributionInformation;
        }

        // TODO: Maybe some kind of CUBE Object inside here?

    }
}
