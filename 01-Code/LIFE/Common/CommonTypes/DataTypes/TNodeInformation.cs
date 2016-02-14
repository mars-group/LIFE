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
using ProtoBuf;

namespace CommonTypes.DataTypes {
    [ProtoContract]
    [Serializable]
    public class TNodeInformation : IComparable {
        [ProtoMember(1)]
        public NodeType NodeType { get; private set; }

        [ProtoMember(2)]
        public string NodeIdentifier { get; private set; }

        [ProtoMember(3)]
        public NodeEndpoint NodeEndpoint { get; private set; }


        private TNodeInformation() {}

        public TNodeInformation(NodeType nodeType, string nodeIdentifier, NodeEndpoint nodeEndpoint) {
            NodeType = nodeType;
            NodeIdentifier = nodeIdentifier;
            NodeEndpoint = nodeEndpoint;
        }

        #region IComparable Members

        public int CompareTo(object obj) {
            return NodeIdentifier.CompareTo((obj as TNodeInformation).NodeIdentifier);
        }

        #endregion

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            return
                sb.AppendFormat
                    ("[NodeIdentifier{0}, TNode{1}, NodeEndpoint{2}]", NodeIdentifier, NodeType, NodeEndpoint)
                    .ToString();
        }


        public override bool Equals(object obj) {


            TNodeInformation otherNodeInfo = obj as TNodeInformation;
            if (otherNodeInfo == null)
            {
                return false;    
            }
            if (otherNodeInfo.NodeIdentifier == null || otherNodeInfo.NodeType == null) {
                return false;
            }

           return (otherNodeInfo.NodeIdentifier.Equals(NodeIdentifier) && otherNodeInfo.NodeType.Equals(NodeType));

        }

        public override int GetHashCode() {
            return NodeIdentifier.GetHashCode()*NodeType.GetHashCode()*347;
        }
    }
}