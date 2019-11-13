using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowR
{
    public class FlowValueDictionary<T> : Dictionary<string, T>, IFlowValueDictionary
    {
        void IFlowValueDictionary.SetValue(string key, object value)
        {
            // TODO: We may get cast exception here, so we should catch it explicitly
            base.Add(key, (T)value);
        }

        IEnumerable<string> IFlowValueDictionary.Keys => this.Keys;

        bool IFlowValueDictionary.TryGetValue(string key, out object value)
        {
            var tryGetValue = this.TryGetValue(key, out var tValue);

            value = tValue;

            return tryGetValue;
        }

        IEnumerable<KeyValuePair<string, object>> IFlowValueDictionary.GetValues(Type targetType)
        {
            var values = new List<KeyValuePair<string, object>>();

            foreach (var key in this.Keys)
            {
                var value = this[key];

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

        object IFlowValueDictionary.this[string key] => this[key];
    }

    public interface IFlowValueDictionary
    {
        void SetValue(string key, object value);

        IEnumerable<string> Keys { get; }

        bool TryGetValue(string key, out object value);

        IEnumerable<KeyValuePair<string, object>> GetValues(Type targetType);

        object this[string key] { get; }
    }
}
