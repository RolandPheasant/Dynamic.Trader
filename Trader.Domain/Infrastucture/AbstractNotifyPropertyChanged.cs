using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TradeExample.Annotations;

namespace Trader.Domain.Infrastucture
{
    public abstract class AbstractNotifyPropertyChanged: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetAndRaise<T>(ref T backingField, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, newValue)) return;
            backingField = newValue;
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(propertyName);
        }


    }
}
