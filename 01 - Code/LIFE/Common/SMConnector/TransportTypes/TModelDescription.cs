// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 06.11.2014
//  *******************************************************/

using System;

namespace SMConnector.TransportTypes {
    [Serializable]
    public class TModelDescription : IEquatable<TModelDescription> {
        public string SourceURL { get; private set; }
        public string Name { get; private set; }

        public bool Running { get; private set; }

        public string Description { get; private set; }

        public TStatusUpdate Status { get; set; }

        public TModelDescription
            (string name,
                string description = "",
                string status = "Not Running",
                bool running = false,
                string sourceURL = "") {
            Name = name;
            Running = running;
            Description = description;
            Status = new TStatusUpdate(status);
            SourceURL = sourceURL;
        }

        public TModelDescription() {}

        #region Object contracts

        public bool Equals(TModelDescription other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TModelDescription) obj);
        }

        public override int GetHashCode() {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(TModelDescription left, TModelDescription right) {
            return Equals(left, right);
        }

        public static bool operator !=(TModelDescription left, TModelDescription right) {
            return !Equals(left, right);
        }

        #endregion
    }
}