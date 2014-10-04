using System;
using GoapCommon.Interfaces;

namespace GoapModelTest.Worldstates {
    public class HasToy : IGoapWorldProperty, IEquatable<HasToy> {

        private bool _isValid;
        private readonly Enum _stateSymbol = WorldStateEnums.HasToy;

        public HasToy(bool valid){
            _isValid = valid;
        }

        public IGoapWorldProperty GetNegative(){
            return new HasToy(false);
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
            return new HasToy(_isValid);
        }

        public override string ToString()
        {
            return string.Format("|{0}:{1}|", _stateSymbol, _isValid);
        }

        public bool Equals(HasToy other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _isValid.Equals(other._isValid) && Equals(_stateSymbol, other._stateSymbol);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HasToy) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (_isValid.GetHashCode()*397) ^ (_stateSymbol != null ? _stateSymbol.GetHashCode() : 0);
            }
        }

        public static bool operator ==(HasToy left, HasToy right) {
            return Equals(left, right);
        }

        public static bool operator !=(HasToy left, HasToy right) {
            return !Equals(left, right);
        }
    }
}