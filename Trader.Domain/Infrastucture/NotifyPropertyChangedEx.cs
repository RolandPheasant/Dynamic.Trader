using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace Trader.Domain.Infrastucture
{

    public sealed class PropertyValue<TObject, TValue> : IEquatable<PropertyValue<TObject, TValue>>
    {
        private readonly TObject _sender;
        private readonly TValue _value;

        public PropertyValue(TObject sender, TValue value)
        {
            _sender = sender;
            _value = value;
        }

        public TObject Sender
        {
            get { return _sender; }
        }

        public TValue Value
        {
            get { return _value; }
        }

        #region Equality

        public bool Equals(PropertyValue<TObject, TValue> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TObject>.Default.Equals(_sender, other._sender) && EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is PropertyValue<TObject, TValue> && Equals((PropertyValue<TObject, TValue>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TObject>.Default.GetHashCode(_sender) * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(_value);
            }
        }

        public static bool operator ==(PropertyValue<TObject, TValue> left, PropertyValue<TObject, TValue> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PropertyValue<TObject, TValue> left, PropertyValue<TObject, TValue> right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} ({1})", _sender, _value);
        }
    }
   
    public static class NotifyPropertyChangedEx
    {
        /// <summary>
        /// Observes property changes for the sepcifed property, starting with the current value
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyAccessor">The property accessor.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">propertyAccessor</exception>
        public static IObservable<PropertyValue<TObject, TValue>> ObservePropertyValue<TObject, TValue>(this TObject source,
            Expression<Func<TObject, TValue>> propertyAccessor)
            where TObject:INotifyPropertyChanged
        {
            if (propertyAccessor == null) throw new ArgumentNullException("propertyAccessor");

            var member = propertyAccessor.GetProperty();
            var accessor = propertyAccessor.Compile();

            Func<PropertyValue<TObject, TValue>> factory =
                () => new PropertyValue<TObject, TValue>(source, accessor(source));

            var propertyChanged =   Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>
                (
                    handler => source.PropertyChanged += handler,
                    handler => source.PropertyChanged -= handler
                )
                .Where(args => args.EventArgs.PropertyName == member.Name)
                .Select(x => factory())
                .StartWith(factory());

            return propertyChanged;
        }



        private static PropertyInfo GetProperty<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            var property = GetMember(expression) as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Not a property expression");

            return property;
        }

        private static MemberInfo GetMember<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            if (expression == null)
                throw new ArgumentException("Not a property expression");

            return GetMemberInfo(expression);
        }

        private static MemberInfo GetMemberInfo(LambdaExpression lambda)
        {
            if (lambda == null)
                throw new ArgumentException("Not a property expression");

            MemberExpression memberExpression = null;
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = lambda.Body as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.Call)
            {
                return ((MethodCallExpression)lambda.Body).Method;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access");
            }

            return memberExpression.Member;
        }


    }
}