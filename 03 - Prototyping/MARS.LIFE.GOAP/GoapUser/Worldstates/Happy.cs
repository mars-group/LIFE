using System;
using System.Collections.Generic;
using CommonTypes.Interfaces;
using GoapCommon.Interfaces;

namespace GoapUser.Worldstates {

    internal class Happy : IGoapWorldstate, IEquatable<Happy> {

        private bool _isValid;
        private readonly Enum _stateSymbol;


        internal Happy(bool valid, Enum stateSymbol) {
            _isValid = valid;
            _stateSymbol = stateSymbol;
        }

        public Enum GetWorldstateSymbol() {
            return _stateSymbol;
        }

        public bool IsValid() {
            return _isValid;
        }

        public void SetIsValid(bool trueOrFalse) {
            _isValid = trueOrFalse;
        }

        public void SwitchIsValid() {
            SetIsValid(this._isValid != true);
        }

        public IGoapWorldstate GetClone() {
            return new Happy(_isValid, _stateSymbol);
        }

        public override string ToString() {
            return string.Format("IsValid: {0}, StateSymbol: {1}", _isValid, _stateSymbol);
        }


        public bool Equals(Happy other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _isValid.Equals(other._isValid) && Equals(_stateSymbol, other._stateSymbol);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Happy) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (_isValid.GetHashCode()*397) ^ (_stateSymbol != null ? _stateSymbol.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Happy left, Happy right) {
            return Equals(left, right);
        }

        public static bool operator !=(Happy left, Happy right) {
            return !Equals(left, right);
        }
    }
}