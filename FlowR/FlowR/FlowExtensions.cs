using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FlowR
{
    public static class FlowExtensions
    {
        public static void RegisterFlowTypes(this Assembly assembly, Action<Type, Type> registerFlowType)
        {
            var flowTypes = assembly.GetExportedTypes().Where(t => typeof(IFlowHandler).IsAssignableFrom(t));

            foreach (var flowType in flowTypes)
            {
                registerFlowType(typeof(IFlowHandler), flowType);
            }
        }
    }
}
