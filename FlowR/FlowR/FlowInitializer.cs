using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlowR
{
    public class FlowInitializer<TReq>
    {
        internal List<FlowValueOutputBinding> Outputs { get; } = new List<FlowValueOutputBinding>();

        public FlowInitializer<TReq> BindValue<TVal>(Expression<Func<TReq, TVal>> propertyExpression, string flowValueName, Func<TVal, object> mapValue = null)
        {
            var propertyInfo = ReflectionUtils.GetProperty(propertyExpression);

            var boundOutputProperty = typeof(TReq).GetFlowObjectType()[propertyInfo.Name];

            var binding =
                new FlowValueOutputBinding(boundOutputProperty)
                {
                    MapName = (n, r) => flowValueName
                };

            if (mapValue != null)
            {
                binding.MapValue = fv => mapValue((TVal)fv);
            }

            Outputs.Add(binding);

            return this;
        }

        public FlowInitializer<TReq> BindOutputs<TVal>(Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression,
            params string[] outputValueNames)
        {
            return BindOutputs(propertyExpression, new FlowValueListSelector(outputValueNames));
        }

        public FlowInitializer<TReq> BindOutputs<TVal>(Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression,
            IDictionary<string, string> nameMap)
        {
            return BindOutputs(propertyExpression, new FlowValueListSelector(nameMap));
        }

        public FlowInitializer<TReq> BindOutputs<TVal>(Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression,
            FlowValueSelector flowValueSelector, Func<TVal, object> mapValue = null)
        {
            var propertyInfo = ReflectionUtils.GetProperty(propertyExpression);

            var boundOutputProperty = typeof(TReq).GetFlowObjectType()[propertyInfo.Name];

            var binding =
                new FlowValueOutputBinding(boundOutputProperty)
                {
                    FlowValueSelector = flowValueSelector
                };

            if (mapValue != null)
            {
                binding.MapValue = fv => mapValue((TVal)fv);
            }

            Outputs.Add(binding);

            return this;
        }
    }
}