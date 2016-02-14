//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;

namespace SMConnector.TransportTypes {
    [Serializable]
	public class TModelDescription : IEquatable<TModelDescription> {

        public string Name { get; private set; }

        public TModelDescription(string name) {
            Name = name;
        }

        public TModelDescription() {}

        #region Object contracts

		public override bool Equals (object obj)
    	{
    		if (obj == null)
    			return false;
    		if (ReferenceEquals (this, obj))
    			return true;
    		if (obj.GetType () != typeof(TModelDescription))
    			return false;
    		TModelDescription other = (TModelDescription)obj;
    		return Name == other.Name;
    	}
    	

    	public override int GetHashCode ()
    	{
    		unchecked {
    			return (Name != null ? Name.GetHashCode () : 0);
    		}
    	}

        #endregion

		#region IEquatable implementation

		public bool Equals (TModelDescription other)
		{
			return this.Name == other.Name;
		}

		#endregion
    }
}