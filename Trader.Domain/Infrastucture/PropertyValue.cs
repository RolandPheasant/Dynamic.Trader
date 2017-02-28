using System;
using System.Collections.Generic;

namespace Trader.Domain.Infrastucture
{
    public sealed class PropertyValue<TObject, TValue> : IEquatable<PropertyValue<TObject, TValue>>
    {
        public PropertyValue(TObject sender, TValue value)
        {
            Sender = sender;
            Value = value;
        }

        public TObject Sender { get; }
        public TValue Value { get; }

        #region Equality

        public bool Equals(PropertyValue<TObject, TValue> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TObject>.Default.Equals(Sender, other.Sender) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
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
                return (EqualityComparer<TObject>.Default.GetHashCode(Sender) * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(Value);
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
            return $"{Sender} ({Value})";
        }
    }
}