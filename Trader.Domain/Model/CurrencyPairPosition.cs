using System;
using System.Linq;
using DynamicData;
using Trader.Domain.Infrastucture;

namespace Trader.Domain.Model
{
    public class CurrencyPairPosition: AbstractNotifyPropertyChanged,  IDisposable, IEquatable<CurrencyPairPosition>
    {
        private readonly IDisposable _cleanUp;
        private readonly string _currencyPair;
        private TradesPosition _position;

        public CurrencyPairPosition(IGroup<Trade, long, string> tradesByCurrencyPair)
        {
            _currencyPair = tradesByCurrencyPair.Key;
            _cleanUp = tradesByCurrencyPair.Cache.Connect()
                .QueryWhenChanged(query =>
                {
                    var buy = query.Items.Where(trade => trade.BuyOrSell == BuyOrSell.Buy).Sum(trade=>trade.Amount);
                    var sell = query.Items.Where(trade => trade.BuyOrSell == BuyOrSell.Sell).Sum(trade => trade.Amount);
                    var count = query.Count;
                    return new TradesPosition(buy,sell,count);
                })
                .Subscribe(position => Position = position);
        }

        public TradesPosition Position
        {
            get { return _position; }
            set { SetAndRaise(ref  _position,value); }
        }

        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        #region Equality Members

        public bool Equals(CurrencyPairPosition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_currencyPair, other._currencyPair);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CurrencyPairPosition) obj);
        }

        public override int GetHashCode()
        {
            return (_currencyPair != null ? _currencyPair.GetHashCode() : 0);
        }

        public static bool operator ==(CurrencyPairPosition left, CurrencyPairPosition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CurrencyPairPosition left, CurrencyPairPosition right)
        {
            return !Equals(left, right);
        }

        #endregion

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}