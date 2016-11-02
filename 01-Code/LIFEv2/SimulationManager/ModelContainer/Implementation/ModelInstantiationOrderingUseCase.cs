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
using LayerLoader.Interface;
using LCConnector.TransportTypes;
using ModelContainer.Implementation.Entities;
using SMConnector.TransportTypes;


namespace ModelContainer.Implementation {
    /// <summary>
    ///     This class reads one specific model and converts it into a representation that allows<br />
    ///     for instantiation order analysis.
    /// </summary>
    internal class ModelInstantiationOrderingUseCase
    {


        public ModelInstantiationOrderingUseCase()
        {
        }


        public IList<TLayerDescription> GetInstantiationOrder(TModelDescription description) {

            ILayerLoader addinLoader = new LayerLoader.Implementation.LayerLoader();

            var nodes = addinLoader.LoadAllLayersForModel(description.ModelPath);

            var modelStructure = new ModelStructure();

            foreach (var node in nodes) {


                var type = node.LayerType;
                var constructors = type.GetConstructors();

                Console.WriteLine($"Found layer: {type.Name}");

                // make sure all parameters in all constructors are Interfaces, throw exception otherwise
                if (
                    constructors.Any(
                        info => info.GetParameters().Any(parameterInfo => !parameterInfo.ParameterType.GetTypeInfo().IsInterface)))
                {
                    throw new AllLayerConstructorParamtersNeedToBeInterfacesException(
                        "Make sure all your parameters in your Layer's constructor are interface types." +
                        "This is required for MARS LIFE's distribution to work properly."
                        );
                }
                var layerDescription = new TLayerDescription
                    (type.Name,
                        type.GetTypeInfo().Assembly.GetName().Version.Major,
                        type.GetTypeInfo().Assembly.GetName().Version.Minor,
                        type.GetTypeInfo().Assembly.Location,
                        type.FullName,
                        type.AssemblyQualifiedName);

                if (constructors.Any(c => c.GetParameters().Length == 0))
                {
                    modelStructure.AddLayer(layerDescription, type);
                }
                else
                {
                    var paramList = constructors.First(c => c.GetParameters().Length > 0).GetParameters();
                    modelStructure.AddLayer(layerDescription, type, paramList.Select(p => p.ParameterType).ToArray());
                }
            }

            var instOrder = modelStructure.CalculateInstantiationOrder();
            Console.Error.WriteLine("Inst Order:");
            foreach (var layerDescription in instOrder)
            {
                Console.WriteLine($"{layerDescription.FullName}");
            }
            return instOrder;
        }
    }


    [Serializable]
    internal class AllLayerConstructorParamtersNeedToBeInterfacesException : Exception
    {
        public AllLayerConstructorParamtersNeedToBeInterfacesException(string s) : base(s)
        {
            
        }
    }
}