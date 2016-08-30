//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Text;
using CommonTypes.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static System.String;

namespace CommonTypes.DataTypes {

    public class TNodeInformation : IComparable {

        [JsonConverter(typeof(StringEnumConverter))]
        public NodeType NodeType { get; private set; }


        public string NodeIdentifier { get; private set; }


        public NodeEndpoint NodeEndpoint { get; private set; }


        private TNodeInformation() {}

        public TNodeInformation(NodeType nodeType, string nodeIdentifier, NodeEndpoint nodeEndpoint) {
            NodeType = nodeType;
            NodeIdentifier = nodeIdentifier;
            NodeEndpoint = nodeEndpoint;
        }

        #region IComparable Members

        public int CompareTo(object obj) {
            return Compare(NodeIdentifier, (obj as TNodeInformation).NodeIdentifier, StringComparison.Ordinal);
        }

        #endregion

        public override string ToString() {
            var sb = new StringBuilder();

            return
                sb.AppendFormat
                    ("[NodeIdentifier: {0}, TNode: {1}, NodeEndpoint: {2}]", NodeIdentifier, NodeType, NodeEndpoint)
                    .ToString();
        }


        public override bool Equals(object obj) {


            var otherNodeInfo = obj as TNodeInformation;
            if (otherNodeInfo?.NodeIdentifier == null) {
                return false;
            }

           return otherNodeInfo.NodeIdentifier.Equals(NodeIdentifier) && otherNodeInfo.NodeType.Equals(NodeType);

        }

        public override int GetHashCode() {
            return NodeIdentifier.GetHashCode()*NodeType.GetHashCode()*347;
        }
    }
}