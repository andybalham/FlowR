using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FlowR
{
    public class FlowObjectProperty
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsNotNullValue { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsDictionaryBinding { get; set; }
        public bool IsOverridableValue { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public Type DictionaryType { get; set; }
        public Type DictionaryValueType { get; set; }
    }

}
