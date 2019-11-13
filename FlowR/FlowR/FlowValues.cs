using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowR
{
    public class FlowValues : IFlowValueDictionary
    {
        private readonly IDictionary<string, object> _values = new Dictionary<string, object>();

        public void SetValue(string name, object value)
        {
            _values[name] = value;
        }

        public bool TryGetValue(string key, out object value)
        {
            return _values.TryGetValue(key, out value);
        }

        public IEnumerable<KeyValuePair<string, object>> GetValues(Type targetType)
        {
            var values = new List<KeyValuePair<string, object>>();

            foreach (var key in this.Keys)
            {
                var value = _values[key];

                if (value == null)
                {
                    continue;
                }

                if (!value.GetType().IsAssignableFrom(targetType))
                {
                    continue;
                }

                values.Add(new KeyValuePair<string, object>(key, value));
            }

            return values;
        }

        public object this[string key] => _values[key];

        public IEnumerable<string> Keys => _values.Keys;
    }
}
