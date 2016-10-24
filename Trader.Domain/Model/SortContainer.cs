using System;
using System.Collections.Generic;

namespace Trader.Domain.Model
{
    public sealed class SortContainer : IEquatable<SortContainer>
    {
        private readonly string _description;

        public SortContainer(string description, IComparer<TradeProxy> comparer)
        {
            _description = description;
            Comparer = comparer;
        }

        public IComparer<TradeProxy> Comparer { get; }

        public string Description => _description;

        #region Equality members

        public bool Equals(SortContainer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_description, other._description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SortContainer)obj);
        }

        public override int GetHashCode()
        {
            return (_description != null ? _description.GetHashCode() : 0);
        }

        public static bool operator ==(SortContainer left, SortContainer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SortContainer left, SortContainer right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}