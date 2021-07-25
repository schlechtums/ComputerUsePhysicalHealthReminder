using CUPHR.ViewModel.Types.Timers;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace CUPHR.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Title += $" ({version?.Major}.{version?.Minor}.{version?.Build})";
        }

        private ViewModel.ViewModel _VM;

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this._VM = ((sender as FrameworkElement).DataContext) as ViewModel.ViewModel;
            this._VM.OnTimerElapsed += this.HandleTimerElapsed;

            this._VM.StartAllTimers();
        }

        private void HandleTimerElapsed(TimerBase sender, String timerElapsedMessage)
        {
            new ToastContentBuilder().AddText($"{sender.Name} elapsed")
                                     .AddText(timerElapsedMessage)
                                     .Show();
        }
    }
}
