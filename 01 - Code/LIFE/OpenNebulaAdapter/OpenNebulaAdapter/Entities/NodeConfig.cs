

using System.Collections.Generic;

namespace OpenNebulaAdapter.Entities
{
    class NodeConfig {

        protected string Name { get; private set; }
        protected string Owner { get; private set; }
        protected List<Node> Nodes { get; private set; }

        public NodeConfig(IDictionary<string,object> jsNodeConfig) {
            Name = (string)jsNodeConfig["name"];
            Owner = (string)jsNodeConfig["owner"];
            foreach (IDictionary<string, dynamic> node in (object[])jsNodeConfig["nodes"])
            {
                Nodes.Add(new Node(node));
            }
        }
    }
}
