using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Trader.Domain.Model
{
    public class Trade: IDisposable, IEquatable<Trade>
    {
        private readonly ISubject<decimal> _marketPriceChangedSubject = new ReplaySubject<decimal>(1); 
        
        public long Id { get; }
        public string CurrencyPair { get; }
        public string Customer { get; }
        public decimal TradePrice { get; }
        public decimal MarketPrice { get; private set; }
        public decimal PercentFromMarket { get; private set; }
        public decimal Amount { get; }
        public BuyOrSell BuyOrSell { get; }
        public TradeStatus Status { get; private set; }
        public DateTime Timestamp { get; private set; }

        public Trade(Trade trade, TradeStatus status)
        {
            Id = trade.Id;
            Customer = trade.Customer;
            CurrencyPair = trade.CurrencyPair;
            Status = status;
            MarketPrice = trade.MarketPrice;
            TradePrice = trade.TradePrice;
            Amount = trade.Amount;
            Timestamp = DateTime.Now;
            BuyOrSell = trade.BuyOrSell;
        }

        public Trade(long id, string customer, string currencyPair, TradeStatus status, BuyOrSell buyOrSell, decimal tradePrice, decimal amount, decimal marketPrice = 0, DateTime? timeStamp = null)
        {
            Id = id;
            Customer = customer;
            CurrencyPair = currencyPair;
            Status = status;
            MarketPrice = marketPrice;
            TradePrice = tradePrice;
            Amount = amount;
            BuyOrSell = buyOrSell;
            Timestamp =timeStamp ?? DateTime.Now;
        }

        public void SetMarketPrice(decimal marketPrice)
        {
            MarketPrice = marketPrice;
            PercentFromMarket = Math.Round(((TradePrice - MarketPrice) / MarketPrice) * 100, 4);
            _marketPriceChangedSubject.OnNext(marketPrice);
        }

        public IObservable<decimal> MarketPriceChanged => _marketPriceChangedSubject.AsObservable();

        #region Equality Members

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