

using System.Collections.Generic;

namespace OpenNebulaAdapter.Entities
{
    public class NodeConfig {

        public string Name { get; private set; }
        public string Owner { get; private set; }
        public List<Node> Nodes { get; private set; }

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
