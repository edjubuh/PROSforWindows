using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace PROSforWindows.Resources
{
    public static class PropertyCoupler
    {
        public static T CoupleProperty<T, U, V>(this T left, Expression<Func<T, V>> leftPropertyExpression,
                                                     U right, Expression<Func<U, V>> rightPropertyExpression)
            where T : INotifyPropertyChanged
            where U : INotifyPropertyChanged
        {
            string leftPropertyName = getPropertyName(leftPropertyExpression);
            string rightPropertyName = getPropertyName(rightPropertyExpression);

            var leftProperty = (PropertyInfo)((MemberExpression)leftPropertyExpression.Body).Member;
            var rightProperty = (PropertyInfo)((MemberExpression)rightPropertyExpression.Body).Member;

            left.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == leftPropertyName)
                {
                    rightProperty.SetValue(right, leftProperty.GetValue(left));
                }
            };

            right.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == rightPropertyName)
                {
                    leftProperty.SetValue(left, rightProperty.GetValue(right));
                }
            };

            return left;
        }

        #region Getting property name from a lambda expression
        private static string getPropertyName<TObservedType, TPropertyType>(Expression<Func<TObservedType, TPropertyType>> propertyExpression) where TObservedType : INotifyPropertyChanged
        {
            var lambda = propertyExpression as LambdaExpression;
            MemberInfo info = getMemberExpression(lambda).Member;
            return info.Name;
        }

        private static MemberExpression getMemberExpression(LambdaExpression lambda)
        {
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else memberExpression = lambda.Body as MemberExpression;
            return memberExpression;
        }

        #endregion
    }
}
