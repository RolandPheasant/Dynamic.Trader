using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace Trader.Client.Views
{
    /// <summary>
    /// Interaction logic for LogEntryView.xaml
    /// </summary>
    public partial class LogEntryView : UserControl, IViewFor<LogEntryViewer>
    {
        public LogEntryView()
        {
            InitializeComponent();
            
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);

        }

        private void CCC(object xx)
        {
            
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (LogEntryViewer), typeof (LogEntryView), new PropertyMetadata(default(LogEntryViewer)));

        public LogEntryViewer ViewModel
        {
            get { return (LogEntryViewer) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (LogEntryViewer)value; }
        }
    }
}
