using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BoundValueAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullValueAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveValueAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OverridableValueAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; }

        public DescriptionAttribute(string description)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class BindingNameAttribute : Attribute
    {
        protected BindingNameAttribute()
        {
        }

        protected BindingNameAttribute(string targetProperty)
        {
            TargetProperty = targetProperty;
        }

        public string TargetProperty { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InputBindingNameAttribute : BindingNameAttribute
    {
        public InputBindingNameAttribute()
        {
        }

        public InputBindingNameAttribute(string targetProperty) : base(targetProperty)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OutputBindingNameAttribute : BindingNameAttribute
    {
        public OutputBindingNameAttribute()
        {
        }

        public OutputBindingNameAttribute(string targetProperty) : base(targetProperty)
        {
        }
    }
}
