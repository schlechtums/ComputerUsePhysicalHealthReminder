using CUPHR.ViewModel.Types;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shell;

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
            this._OriginalTitle = this.Title;
        }

        private ViewModel.ViewModel _VM;
        private String _OriginalTitle;

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this._VM = ((sender as FrameworkElement).DataContext) as ViewModel.ViewModel;
            this._VM.OnTimerElapsed += this.HandleTimerElapsed;

            this._VM.PropertyChanged += this.HandleViewModelPropertyChanged;

            this._VM.StartAllTimers();
        }

        private void HandleTimerElapsed(Timer sender, String timerElapsedMessage)
        {
            new ToastContentBuilder().AddText($"{sender.Name} elapsed")
                                     .AddText(timerElapsedMessage)
                                     .SetToastScenario(ToastScenario.Reminder)
                                     .Show();
        }

        private void HandleViewModelPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.ViewModel.NextTimerToExpire))
                this.HandleNextTimerToExpire(this._VM.NextTimerToExpire);
        }

        private void HandleNextTimerToExpire(Timer t)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Title = $"{this._OriginalTitle} ({t.Name} - {t.TimeRemaining.ToString(@"hh\:mm\:ss")})";
                this.TaskbarItemInfo = new TaskbarItemInfo
                {
                    ProgressState = TaskbarItemProgressState.Normal,
                    ProgressValue = 1d - ((double)t.TimeRemaining.TotalSeconds / t.Interval.TotalSeconds)
                };
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.TaskbarItemInfo = new TaskbarItemInfo();
        }
    }
}
