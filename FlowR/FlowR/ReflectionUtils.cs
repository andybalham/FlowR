using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FlowR
{
    internal class ReflectionUtils
    {
        public static PropertyInfo GetProperty<TR, TV>(Expression<Func<TR, TV>> propertyExpression)
        {
            var propertyExpressionMemberInfo = GetMemberInfo(propertyExpression.Body);

            if (propertyExpressionMemberInfo.MemberType != MemberTypes.Property)
            {
                throw new FlowException($"The member {propertyExpressionMemberInfo.Name} on type {typeof(TR).FullName} is not a property as is required.");
            }

            var propertyInfo = (PropertyInfo)propertyExpressionMemberInfo;

            return propertyInfo;
        }

        private static MemberInfo GetMemberInfo(Expression expression)
        {
            switch (expression)
            {
                case null:
                    throw new ArgumentNullException(nameof(expression));

                // Reference type property or field
                case MemberExpression memberExpression:
                    return memberExpression.Member;

                // Property, field of method returning value type
                case UnaryExpression unaryExpression:
                    return GetMemberInfo(unaryExpression);

                default:
                    throw new ArgumentException("Invalid property expression");
            }
        }

        private static MemberInfo GetMemberInfo(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression methodExpression)
            {
                return methodExpression.Method;
            }

            return ((MemberExpression)unaryExpression.Operand).Member;
        }
    }
}
