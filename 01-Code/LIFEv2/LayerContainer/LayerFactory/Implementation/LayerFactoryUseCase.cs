//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using LayerFactory.Interface;
using LayerLoader.Interface;
using LayerRegistry.Interfaces;
using LCConnector.TransportTypes.ModelStructure;
using LifeAPI.Layer;
using Microsoft.Extensions.DependencyModel;


namespace LayerFactory.Implementation {
    internal class LayerFactoryUseCase : ILayerFactory {
        private readonly ILayerRegistry _layerRegistry;
        private readonly ILayerLoader _layerLoader;

        public LayerFactoryUseCase(ILayerRegistry layerRegistry) {
            _layerRegistry = layerRegistry;
            _layerLoader = new LayerLoader.Implementation.LayerLoader();
        }

        #region ILayerFactory Members

        public static IEnumerable<Assembly> GetReferencingAssemblies(string assemblyName)
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateLibrary(library, assemblyName))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
        {
            return library.Name == (assemblyName)
                || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }



        public ILayer GetLayer(string layerName) {
            ILayer result;

            var layerTypeInfo = _layerLoader.LoadLayerOnLayerContainer(layerName);

            var constructors = layerTypeInfo.Constructors;

            var references = GetReferencingAssemblies(layerTypeInfo.LayerType.GetTypeInfo().Assembly.GetName().Name);

            // check if there is an empty constructor
            if (constructors.Any(c => c.GetParameters().Length == 0))
            {
                var ctor = constructors.First(c => c.GetParameters().Length == 0);

                result = (ILayer) Activator.CreateInstance(layerTypeInfo.LayerType);
                //result = (ILayer) ctor.Invoke(new object[0]);
            }
            else {
                // take first constructor, resolve dependencies from LayerRegistry and instantiate Layer
                var currentConstructor = constructors[0];
                var neededParameters = currentConstructor.GetParameters();

                var actualParameters = new object[neededParameters.Length];

                var i = 0;
                foreach (var parameterInfo in neededParameters) {
                    //Console.WriteLine($"Trying to resolve Layer Param: {parameterInfo.ParameterType.FullName}");
                    var param = _layerRegistry.GetLayerInstance(parameterInfo.ParameterType);
                    /*Console.WriteLine($"Got param: {param.GetType().FullName}");
                    //Console.WriteLine($"Param {param.GetType().FullName} implements the following interfaces:");
                    foreach (var inf in param.GetType().GetInterfaces())
                    {
                        Console.WriteLine($"Inf: {inf.FullName}");
                    }
                    */

                    actualParameters[i] = param;
                    i++;
                }
                result = null;
                try
                {
                    result = (ILayer) Activator.CreateInstance(layerTypeInfo.LayerType, actualParameters);
                    //result = (ILayer) currentConstructor.Invoke(actualParameters.ToArray());
                }
                catch (MissingMethodException mex)
                {
                    var stb = new StringBuilder();
                    foreach (var actualParameter in actualParameters)
                    {
                        stb.Append(actualParameter.GetType().FullName);
                        stb.Append(" | ");
                    }
                    Console.Error.WriteLine($"Something was wrong with the parameters for Layer {layerTypeInfo.LayerType.FullName}. Parameters were: " + stb);
                    throw mex;
                }
             }
            _layerRegistry.RegisterLayer(result);
            return result;
        }

        public void LoadModelContent(ModelContent content) {
            _layerLoader.LoadModelContent(content);
        }

        #endregion
    }
}