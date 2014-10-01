using System;
using System.IO;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using LayerContainerFacade.Interfaces;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;
using LayerContainerShared;



namespace LayerContainerFacade.Implementation
{


    internal class LayerContainerFacadeImpl : ScsService, ILayerContainerFacade
    {
        private readonly IPartitionManager _partitionManager;
        private readonly IRTEManager _rteManager;
        private IScsServiceApplication _server;

        public LayerContainerFacadeImpl(LayerContainerSettings settings, IPartitionManager partitionManager, IRTEManager rteManager)
        {
            _partitionManager = partitionManager;
            _rteManager = rteManager;

            // empty layers folder
            //EmptyDirectory("./layers");

            _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(settings.NodeRegistryConfig.NodeEndPointPort));

            _server.AddService<ILayerContainer, LayerContainerFacadeImpl>(this);

            //Start server
            _server.Start();
        }

        public void LoadModelContent(ModelContent content)
        {
            _partitionManager.LoadModelContent(content);
        }

        public void Instantiate(TLayerInstanceId instanceId)
        {
            _partitionManager.AddLayer(instanceId);
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData)
        {
            _rteManager.InitializeLayer(instanceId, initData);
        }

        public long Tick()
        {
            return _rteManager.AdvanceOneTick();
        }

        private static void EmptyDirectory(string targetDirectory)
        {
            var dirInfo = new DirectoryInfo(targetDirectory);

            foreach (var file in dirInfo.GetFiles())
            {
                if (!WaitForFile(file.FullName))
                    throw new IOException(string.Format("Could not delete {0} because it is used by someone else.", file.FullName));
                file.Delete();
            }
            foreach (var dir in dirInfo.GetDirectories())
            {
                if (!WaitForFile(dir.FullName))
                    throw new IOException(string.Format("Could not delete {0} because it is used by someone else.", file.FullName));
                dir.Delete(true);
            }


        }

        /// <summary>
        /// Blocks until the file is not locked any more.
        /// </summary>
        /// <param name="fullPath"></param>
        private static bool WaitForFile(string fullPath)
        {
            int numTries = 0;
            while (true)
            {
                ++numTries;
                try
                {
                    // Attempt to open the file exclusively.
                    using (FileStream fs = new FileStream(fullPath,
                        FileMode.Open, FileAccess.ReadWrite,
                        FileShare.None, 100))
                    {
                        fs.ReadByte();

                        // If we got this far the file is ready
                        break;
                    }
                }
                catch (Exception ex)
                {


                    if (numTries > 10)
                    {

                        return false;
                    }

                    // Wait for the lock to be released
                    System.Threading.Thread.Sleep(500);
                }
            }

            return true;
        }
    }
}