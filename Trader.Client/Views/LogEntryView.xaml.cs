using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;

namespace Trader.Client.Views
{
    /// <summary>
    /// Interaction logic for LogEntryView.xaml
    /// </summary>
    public partial class LogEntryView : UserControl, IViewFor<LogEntryViewer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntryView"/> class.
        /// </summary>
        public LogEntryView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);

            this.Bind(ViewModel, model => model.SearchText, view => view.SearchTextBox.Text);


            this.OneWayBind(ViewModel, model => model.DeleteCommand, view => view.DeleteButton.Command);
          //  this.BindCommand(ViewModel, model => model.DeleteCommand, view => view.DeleteButton);
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
