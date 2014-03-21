using System;
using Hik.Communication.Scs.Communication.Messages;

namespace Hik.Communication.ScsServices.Communication.Messages
{
    [Serializable]
    public class PropertyUpdatedMessage : ScsMessage
    {
        public object NewValue { get; set; }
        public string PropertyName { get; set; }


        public PropertyUpdatedMessage(object newValue, string propertyName)
        {
            NewValue = newValue;
            PropertyName = propertyName;
        }

        public override string ToString()
        {
            return string.Format("PropertyUpdatedMessage: {0}.{1}(...)", NewValue, PropertyName);
        }
    }
}
