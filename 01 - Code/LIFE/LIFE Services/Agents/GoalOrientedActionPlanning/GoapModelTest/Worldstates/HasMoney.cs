using System;
using GoapCommon.Interfaces;

namespace GoapModelTest.Worldstates {
    public class HasMoney : IGoapWorldProperty, IEquatable<HasMoney> {
         private bool _isValid;
         private readonly Enum _stateSymbol = WorldStateEnums.HasMoney;

        public HasMoney(bool valid){
            _isValid = valid;
         }
       
        public Enum GetPropertyKey() {
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

        public IGoapWorldProperty GetClone()
        {
            return new HasMoney(_isValid);
        }

        public override string ToString()
        {
            return string.Format("|{0}:{1}|", _stateSymbol, _isValid);
        }

        public bool Equals(HasMoney other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_stateSymbol, other._stateSymbol) && _isValid.Equals(other._isValid);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HasMoney) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((_stateSymbol != null ? _stateSymbol.GetHashCode() : 0)*397) ^ _isValid.GetHashCode();
            }
        }

        public static bool operator ==(HasMoney left, HasMoney right) {
            return Equals(left, right);
        }

        public static bool operator !=(HasMoney left, HasMoney right) {
            return !Equals(left, right);
        }
    }
}