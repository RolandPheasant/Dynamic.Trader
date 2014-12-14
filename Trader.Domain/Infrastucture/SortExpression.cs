using System;

namespace Trader.Domain.Infrastucture
{
    public class SortExpression<T>
    {
        private readonly SortDirection _direction;
        private readonly Func<T, IComparable> _expression;

        public SortExpression(Func<T, IComparable> expression, SortDirection direction = SortDirection.Ascending)
        {
            _expression = expression;
            _direction = direction;
        }

        public SortDirection Direction
        {
            get { return _direction; }
        }

        public Func<T, IComparable> Expression
        {
            get { return _expression; }
        }
    }
}