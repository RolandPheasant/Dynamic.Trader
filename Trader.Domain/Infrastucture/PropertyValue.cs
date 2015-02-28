using System;
using System.Collections.Generic;

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
}