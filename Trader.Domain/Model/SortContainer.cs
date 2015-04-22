using System;
using System.Collections.Generic;

namespace Trader.Domain.Model
{
    public sealed class SortContainer : IEquatable<SortContainer>
    {
        private readonly IComparer<TradeProxy> _comparer;
        private readonly string _description;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SortContainer(string description, IComparer<TradeProxy> comparer)
        {
            _description = description;
            _comparer = comparer;
        }

        public IComparer<TradeProxy> Comparer
        {
            get { return _comparer; }
        }

        public string Description
        {
            get { return _description; }
        }

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