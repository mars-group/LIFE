using System.IO;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.API.Layer.RawFile;
using LIFE.Components.Utilities.CloudSupport;

namespace LIFE.Components.RawFileLayer
{
    public abstract class RawFileLayer : ILayer, IRawFileLayer
    {
        protected string FilePath;
        protected long CurrentTick;

        public FileStream GetFileStream()
        {
            return File.Create(FilePath);
        }

        /// <summary>
        /// This method is invoked after the file was downloaded and saved locally
        /// to initialize the layer with data from the file.
        /// Also general initialization can be placed here.
        /// </summary>
        /// <returns>If the initialization was successfull</returns>
        protected abstract bool AfterInit();

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
            var fileId = layerInitData.FileInitInfo.FileId;
            FilePath = new FileClient().DownloadFile(fileId);

            return AfterInit();
        }

        public long GetCurrentTick()
        {
            return CurrentTick;
        }

        public void SetCurrentTick(long currentTick)
        {
            CurrentTick = currentTick;
        }
    }
}