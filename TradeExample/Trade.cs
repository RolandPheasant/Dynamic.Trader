using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TradeExample
{
    public class Trade: IDisposable, IEquatable<Trade>
    {
        private readonly ISubject<decimal> _marketPriceChangedSubject = new ReplaySubject<decimal>(1); 


        public long Id { get; private set; }
        public string CurrencyPair { get; private set; }
        public string Customer { get; private set; }
        public TradeStatus Status { get; private set; }
        public DateTime Timestamp { get; private set; }
        public decimal TradePrice { get; private set; }
        public decimal MarketPrice { get; private set; }
        public decimal PercentFromMarket { get; private set; }

        public Trade(long id, string customer, string currencyPair, TradeStatus status,decimal tradePrice, decimal marketPrice=0)
        {
            Id = id;
            Customer = customer;
            CurrencyPair = currencyPair;
            Status = status;
            MarketPrice = marketPrice;
            TradePrice = tradePrice;
            Timestamp = DateTime.Now;
        }

        public void SetMarketPrice(decimal marketPrice)
        {
            MarketPrice = marketPrice;
            PercentFromMarket = Math.Round((TradePrice - MarketPrice / ((TradePrice + MarketPrice) / 2)) * 100,2); 
            ;
            _marketPriceChangedSubject.OnNext(marketPrice);
        }

        public IObservable<decimal> MarketPriceChanged
        {
            get { return _marketPriceChangedSubject.AsObservable(); }
        }

        #region EqualityMembers

        public bool Equals(Trade other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Trade) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Trade left, Trade right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Trade left, Trade right)
        {
            return !Equals(left, right);
        }

        #endregion

        public void Dispose()
        {
            _marketPriceChangedSubject.OnCompleted();
        }
    }
}