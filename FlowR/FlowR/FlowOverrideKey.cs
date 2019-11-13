using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    public class FlowOverrideKey
    {
        public string Value { get; }
        public string Description { get; }

        public FlowOverrideKey(string value, string description = null)
        {
            Value = value;
            Description = description;
        }
    }
}
