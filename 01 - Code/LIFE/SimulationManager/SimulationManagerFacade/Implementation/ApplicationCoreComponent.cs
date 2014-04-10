using System;
using System.Collections.Generic;
using Hik.Communication.ScsServices.Service;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;
using SimulationManagerFacade.Interface;
using SMConnector;
using SMConnector.TransportTypes;

namespace SimulationManagerFacade.Implementation {
    /// <summary>
    ///     This class represents the implementation of the simulation manager's application core.
    /// </summary>
    /// <remarks>
    ///     It forces the instantiation of all the program's components by requiering them in it's constructor.<br />
    ///     It does, in compliance with the QUASAR pattern mostly delegate calls to the internal components <br />
    ///     (If it is allowed to access them from the outside). Also one would find something like transaction-<br />
    ///     or access controls here. So far this is not yet implemented.
    /// </remarks>
    public class ApplicationCoreComponent : ScsService, IApplicationCore {
        private readonly IRuntimeEnvironment _runtimeEnvironment;
        private readonly IModelContainer _modelContainer;
        private readonly INodeRegistry _nodeRegistry;

        public ApplicationCoreComponent(IRuntimeEnvironment runtimeEnvironment,
                                        IModelContainer modelContainer,
                                        INodeRegistry nodeRegistry) {
            _runtimeEnvironment = runtimeEnvironment;
            _modelContainer = modelContainer;
            _nodeRegistry = nodeRegistry;
        }
        
        #region RuntimeEnvironment delegation

        public void StartSimulationWithModel(TModelDescription model)
        {
            throw new NotImplementedException();
        }

        public void PauseSimulation(TModelDescription model)
        {
            throw new NotImplementedException();
        }

        public void ResumeSimulation(TModelDescription model)
        {
            throw new NotImplementedException();
        }

        public void AbortSimulation(TModelDescription model)
        {
            throw new NotImplementedException();
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ModelContainer delegation

        public void RegisterForModelListChange(Action callback)
        {
            _modelContainer.RegisterForModelListChange(callback);
        }

        public ICollection<TModelDescription> GetAllModels()
        {
            return _modelContainer.GetAllModels();
        }

        IList<TModelDescription> ISimulationManager.GetAllModels()
        {
            throw new NotImplementedException();
        }

        public ModelContent GetModel(TModelDescription modelID)
        {
            return _modelContainer.GetModel(modelID);
        }

        public TModelDescription AddModelFromDirectory(string filePath)
        {
            return _modelContainer.AddModelFromDirectory(filePath);
        }

        public void DeleteModel(TModelDescription model)
        {
            _modelContainer.DeleteModel(model);
        }

        #endregion

        public IList<TLayerDescription> GetInstantiationOrder(TModelDescription model) {
            return _modelContainer.GetInstantiationOrder(model);
        }
    }
}