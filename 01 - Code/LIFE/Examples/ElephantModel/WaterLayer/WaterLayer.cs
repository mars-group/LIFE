using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace WaterLayer
{
    class WaterLayer : ISteppedLayer
    {

    }
}
