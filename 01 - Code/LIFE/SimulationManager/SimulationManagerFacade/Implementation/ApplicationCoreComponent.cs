using System;
using System.Collections;
using System.Collections.Generic;
using CommonTypes.TransportTypes;
using Hik.Communication.ScsServices.Service;
using LCConnector.TransportTypes.ModelStructure;
using ModelContainer.Interfaces;
using RuntimeEnvironment.Interfaces;
using SimulationManagerController.Interfaces;
using SimulationManagerFacade.Interface;
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
        private readonly ISimulationManagerController _simulationManagerController;
        private readonly IRuntimeEnvironment _runtimeEnvironment;
        private readonly IModelContainer _modelContainer;

        public ApplicationCoreComponent(ISimulationManagerController simulationManagerController,
            IRuntimeEnvironment runtimeEnvironment,
            IModelContainer modelContainer) {
            _simulationManagerController = simulationManagerController;
            _runtimeEnvironment = runtimeEnvironment;
            _modelContainer = modelContainer;
        }


        public void RegisterForModelListChange(Action callback) {
            _modelContainer.RegisterForModelListChange(callback);
        }

        public ICollection<TModelDescription> GetAllModels() {
            return _modelContainer.GetAllModels();
        }

        public ModelContent GetModel(TModelDescription modelID) {
            return _modelContainer.GetModel(modelID);
        }

        public TModelDescription AddModelFromDirectory(string filePath) {
            return _modelContainer.AddModelFromDirectory(filePath);
        }

        public void DeleteModel(TModelDescription model) {
            _modelContainer.DeleteModel(model);
        }
    }
}