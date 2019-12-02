using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlowR
{
    public class FlowFinalizer<TRes>
    {
        #region Member declarations

        internal List<FlowValueInputBinding> Inputs { get; } = new List<FlowValueInputBinding>();

        #endregion

        #region Public methods

        public FlowFinalizer<TRes> BindValue<TVal>(Expression<Func<TRes, TVal>> propertyExpression, string flowValueName)
        {
            AddInputBinding(propertyExpression, flowValueName, mapValue: (Func<object, TVal>)null);
            return this;
        }

        public FlowFinalizer<TRes> BindValue<TFlowVal, TVal>(
            Expression<Func<TRes, TVal>> propertyExpression, string flowValueName, Func<TFlowVal, TVal> mapValue)
        {
            AddInputBinding(propertyExpression, flowValueName, mapValue);
            return this;
        }


        public FlowFinalizer<TRes> BindValues<TVal>(
            Expression<Func<TRes, FlowValueDictionary<TVal>>> propertyExpression, params string[] flowValueNames)
        {
            return BindValues(propertyExpression, new FlowValueListSelector(flowValueNames), (Func<object, TVal>)null);
        }

        public FlowFinalizer<TRes> BindValues<TVal>(
            Expression<Func<TRes, FlowValueDictionary<TVal>>> propertyExpression, IDictionary<string, string> nameMap)
        {
            return BindValues(propertyExpression, new FlowValueListSelector(nameMap), (Func<object, TVal>)null);
        }

        public FlowFinalizer<TRes> BindValues<TVal>(
            Expression<Func<TRes, FlowValueDictionary<TVal>>> propertyExpression, FlowValueSelector flowValueSelector)
        {
            return BindValues(propertyExpression, flowValueSelector, (Func<object, TVal>)null);
        }

        public FlowFinalizer<TRes> BindValues<TVal, TFlowVal>(
            Expression<Func<TRes, FlowValueDictionary<TVal>>> propertyExpression, FlowValueSelector flowValueSelector,
            Func<TFlowVal, TVal> mapValue)
        {
            AddInputBinding(propertyExpression, flowValueSelector, mapValue);
            return this;
        }

        #endregion

        #region Private methods

        private void AddInputBinding<TVal, TFlowVal>(
            Expression<Func<TRes, TVal>> propertyExpression, string flowValueName, Func<TFlowVal, TVal> mapValue)
        {
            var boundInputProperty = GetBoundInputProperty(propertyExpression);

            var binding = new FlowValueInputBinding(boundInputProperty)
            {
                FlowValueSelector = new FlowValueSingleSelector(flowValueName),
                MapValue = mapValue == null ? (Func<object, object>)null : v => mapValue((TFlowVal)v)
            };

            Inputs.Add(binding);
        }

        protected void AddInputBinding<TRes, TVal, TFlowVal>(Expression<Func<TRes, FlowValueDictionary<TVal>>> propertyExpression,
            FlowValueSelector flowValueSelector, Func<TFlowVal, TVal> mapValue)
        {
            var boundInputProperty = GetBoundInputProperty(propertyExpression);

            var binding = new FlowValueInputBinding(boundInputProperty)
            {
                FlowValueSelector = flowValueSelector,
                MapValue = mapValue == null ? (Func<object, object>)null : v => mapValue((TFlowVal)v)
            };

            Inputs.Add(binding);
        }

        private static FlowObjectProperty GetBoundInputProperty<TR, TV>(Expression<Func<TR, TV>> propertyExpression)
        {
            var propertyInfo = ReflectionUtils.GetProperty(propertyExpression);

            var flowObjectProperty = typeof(TR).GetFlowObjectType()[propertyInfo.Name];

            return flowObjectProperty;
        }

        #endregion
    }
}