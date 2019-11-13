using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FlowR
{
    public static class FlowObjectTypeCache
    {
        private static readonly object CacheLock = new object();
        private static readonly IDictionary<Type, FlowObjectType> Cache = new Dictionary<Type, FlowObjectType>();

        public static FlowObjectType GetFlowObjectType(this Type type)
        {
            lock (CacheLock)
            {
                if (Cache.TryGetValue(type, out var flowObjectProperties))
                {
                    return flowObjectProperties;
                }

                var newFlowObjectProperties = new FlowObjectType(type);
                Cache.Add(type, newFlowObjectProperties);
                return newFlowObjectProperties;
            }
        }
    }
}
