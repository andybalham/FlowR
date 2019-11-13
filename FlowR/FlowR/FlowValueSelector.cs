using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FlowR
{
    public abstract class FlowValueSelector
    {
        public abstract string[] ResolveNames(object request, IFlowValueDictionary flowValues);

        public virtual Func<string, string> MapName { get; }

        public abstract IEnumerable<Tuple<string, string>> GetSummaries(IFlowStepRequest request);
    }

    public class FlowValueSingleSelector : FlowValueSelector
    {
        private readonly string _name;

        public FlowValueSingleSelector(string name)
        {
            _name = name;
        }

        public override string[] ResolveNames(object request, IFlowValueDictionary flowValues) => new[] { _name };

        public override IEnumerable<Tuple<string, string>> GetSummaries(IFlowStepRequest request)
        {
            return new[] { new Tuple<string, string>(_name, null),  };
        }
    }

    public class FlowValuePropertySelector : FlowValueSelector
    {
        private readonly string _propertyName;
        private readonly Func<object, object> _getPropertyValue;

        public FlowValuePropertySelector(string propertyName, Func<object, object> getPropertyValue)
        {
            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            _getPropertyValue = getPropertyValue ?? throw new ArgumentNullException(nameof(getPropertyValue));
        }

        public override string[] ResolveNames(object request, IFlowValueDictionary flowValues)
        {
            return new[] { (string)_getPropertyValue(request) };
        }

        public override IEnumerable<Tuple<string, string>> GetSummaries(IFlowStepRequest request)
        {
            var property = request.GetType().GetProperty(_propertyName);

            var name = (string)property?.GetValue(request);

            return new[] { new Tuple<string, string>(name, null), };
        }
    }

    public class FlowValueTypeSelector : FlowValueSelector
    {
        public Type TargetType { get; set; }

        public FlowValueTypeSelector(Type targetType, Func<string, string> mapName = null)
        {
            this.TargetType = targetType;
            this.MapName = mapName;
        }

        public override string[] ResolveNames(object request, IFlowValueDictionary flowValues)
        {
            var flowValuesOfTargetType = flowValues.GetValues(this.TargetType);
            var names = flowValuesOfTargetType.Select(fv => fv.Key).ToArray();
            return names;
        }

        public override Func<string, string> MapName { get; }

        public override IEnumerable<Tuple<string, string>> GetSummaries(IFlowStepRequest request)
        {
            return new[]
            {
                new Tuple<string, string>($"Is({this.TargetType.Name})", this.MapName == null ? null : "Func(Name)"),
            };
        }
    }

    public class FlowValueListSelector : FlowValueSelector
    {
        private readonly string[] _names;
        private readonly IDictionary<string, string> _nameMap;

        public FlowValueListSelector(params string[] names)
        {
            _names = names;
        }

        public FlowValueListSelector(IDictionary<string, string> nameMap)
        {
            _names = nameMap.Keys.ToArray();
            _nameMap = nameMap;
            this.MapName = n => nameMap[n];
        }

        public override string[] ResolveNames(object request, IFlowValueDictionary flowValues) => _names;

        public override Func<string, string> MapName { get; }

        public override IEnumerable<Tuple<string, string>> GetSummaries(IFlowStepRequest request)
        {
            var summaries = new List<Tuple<string, string>>();

            summaries.AddRange(this.MapName == null
                ? _names.Select(n => new Tuple<string, string>(n, null))
                : _nameMap.Select(nm => new Tuple<string, string>(nm.Key, nm.Value)));

            return summaries;
        }
    }

    public class FlowValueRegexSelector : FlowValueSelector
    {
        private readonly string _namePattern;

        public FlowValueRegexSelector(string namePattern, Func<string, string> mapName = null)
        {
            _namePattern = namePattern;
            this.MapName = mapName;
        }

        public override string[] ResolveNames(object request, IFlowValueDictionary flowValues)
        {
            var flowValueNames = flowValues.Keys.Where(k => Regex.IsMatch(k, _namePattern)).ToArray();
            return flowValueNames;
        }

        public override Func<string, string> MapName { get; }

        public override IEnumerable<Tuple<string, string>> GetSummaries(IFlowStepRequest request)
        {
            return this.MapName == null
                ? new[] { new Tuple<string, string>($"Match({_namePattern})", null), }
                : new[] { new Tuple<string, string>($"Match({_namePattern})", "Func(Name)"), };
        }
    }
}
