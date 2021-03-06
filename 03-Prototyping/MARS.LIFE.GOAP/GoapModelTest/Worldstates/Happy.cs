﻿using System;
using GoapCommon.Interfaces;

namespace GoapModelTest.Worldstates {
    public class Happy : IGoapWorldstate, IEquatable<Happy> {

        private bool _isValid;
        private readonly Enum _stateSymbol = WorldStateEnums.Happy;


        public Happy(bool valid) {
            _isValid = valid;
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
            return new Happy(_isValid);
        }

        public override string ToString() {
            return string.Format("|{0}:{1}|", _stateSymbol,_isValid);
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