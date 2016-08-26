using System;
using System.Reflection;
using LifeAPI.Layer;

namespace LayerLoader.Implementation
{
    public class LayerTypeInfo
    {
        public LayerTypeInfo(Type layerType, ConstructorInfo[] ctors)
        {
            LayerType = layerType;
            Constructors = ctors;
        }

        public Type LayerType { get; private set; }

        public ConstructorInfo[] Constructors { get; private set; }

    }
}
