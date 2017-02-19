//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using LCConnector.TransportTypes;


public class TLayerInstanceId : IEquatable<TLayerInstanceId> {
        /// <summary>
        ///     The layer's identity.
        /// </summary>
        public TLayerDescription LayerDescription { get; set; }

        /// <summary>
        ///     The instance's unique number.
        /// </summary>
        public int InstanceNr { get; set; }

        public TLayerInstanceId(TLayerDescription layerDescription, int instanceNr) {
            LayerDescription = layerDescription;
            InstanceNr = instanceNr;
        }

        /// <summary>
        ///     ONLY for serializeation
        /// </summary>
        public TLayerInstanceId() {}

        #region Object Contracts

        public bool Equals(TLayerInstanceId other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return LayerDescription.Equals(other.LayerDescription) && InstanceNr == other.InstanceNr;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TLayerInstanceId) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (LayerDescription.GetHashCode()*397) ^ InstanceNr;
            }
        }

        #endregion
}
