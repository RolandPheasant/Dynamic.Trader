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
    /// Interaction logic for RxUiView.xaml
    /// </summary>
    public partial class RxUiView : UserControl,IViewFor<RxUiViewer>
    {
        public RxUiView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);

        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof (RxUiViewer), typeof (RxUiView), new PropertyMetadata(default(RxUiViewer)));

        public RxUiViewer ViewModel
        {
            get { return (RxUiViewer) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (RxUiViewer)value; }
        }
    }
}
