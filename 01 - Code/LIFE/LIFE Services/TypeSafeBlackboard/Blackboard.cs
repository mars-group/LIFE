using System.Collections.Generic;
using System.ComponentModel;

namespace TypeSafeBlackboard {
    /// <summary>
    ///     Licence: CPOL 
    ///     auhor Antonio Nakić Alfirević 
    ///     published at http://www.codeproject.com/Articles/451326/Type-safe-blackboard-property-bag
    /// </summary>
    public class Blackboard : INotifyPropertyChanged, INotifyPropertyChanging {
        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();

        public T Get<T>(BlackboardProperty<T> property) {
            if (!_dict.ContainsKey(property.Name))
                _dict[property.Name] = property.GetDefault();
            return (T) _dict[property.Name];
        }

        public void Set<T>(BlackboardProperty<T> property, T value) {
            OnPropertyChanging(property.Name);
            _dict[property.Name] = value;
            OnPropertyChanged(property.Name);
        }

        #region property change notification

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        protected virtual void OnPropertyChanging(string propertyName) {
            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}