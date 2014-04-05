using Hik.Communication.ScsServices.Service;
using LCConnector.TransportTypes;

namespace LCConnector {
    [ScsService(Version = "0.1")]
    public interface ILayerContainer {
        /// <summary>
        ///     Orders the LayerContainer to instantiate a version of the given layer id. If there already was an instance of that
        ///     type instantiated, it will be overwritten by the new one.
        /// </summary>
        /// <param name="instanceId">A simulation wide unique layer identification.</param>
        /// <param name="layerBinary">the binary code of the layer.</param>
        void Instantiate(TLayerInstanceId instanceId, byte[] layerBinary);

        /// <summary>
        /// Initializes the layer with the given id with the given init data.
        /// </summary>
        /// <param name="instanceId">not null</param>
        /// <param name="initData">not null</param>
        /// <exception cref="Exceptions.LayerNotInstantiatedException">If the layer was not yet instantiated.</exception>
        void InitializeLayer(TLayerInstanceId instanceId, TInitData initData);

        /// <summary>
        /// Calculate one simulation step.
        /// </summary>
        /// <returns>The duration of the tick execution in milliseconds.</returns>
        /// <exception cref="Exceptions.LayerNotInitializedException">If one of the layers not yet initialized.</exception>
        long Tick();
    }
}