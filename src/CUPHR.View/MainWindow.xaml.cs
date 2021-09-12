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
using Windows.ApplicationModel.Activation;

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

            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;
        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            var argsSplit = e.Argument.Split(',');
            var t = this._VM.Timers.Single(t => t.Name == argsSplit[0]);
            if (argsSplit[1] == "StartAction")
                t.StartActionTimer();
            else if (argsSplit[1] == "SkipAction")
                t.SkipActionTimer();
        }

        private ViewModel.ViewModel _VM;
        private String _VersionString;
        private String _OriginalTitle;

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this._VM = ((sender as FrameworkElement).DataContext) as ViewModel.ViewModel;
            this._VM.OnTimerElapsed += this.HandleTimerElapsed;

            this._VM.PropertyChanged += this.HandleViewModelPropertyChanged;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            this._VersionString = $"{version?.Major}.{ version?.Minor}.{ version?.Build}";
            this.Title += $" v{this._VersionString}";
            this._OriginalTitle = this.Title;

            this._VM.StartAllTimers();
        }

        private void HandleTimerElapsed(Timer sender, String timerElapsedMessage)
        {
            var header = sender.IsActionTimer ? $"{sender.Name} completed!" : $"{sender.Name} elapsed";
            var tcb = new ToastContentBuilder().AddText(header)
                                               .AddText(timerElapsedMessage)
                                               .SetToastScenario(ToastScenario.Reminder);

            if (sender.HasAction)
            {
                tcb.SetBackgroundActivation()
                   .AddButton("Start", ToastActivationType.Foreground, $"{sender.Name},StartAction")
                   .AddButton("Skip", ToastActivationType.Foreground, $"{sender.Name},SkipAction");
            }

            if (sender.HasIcon)
            {
                tcb.AddAppLogoOverride(sender.IconUriToast);
            }

            tcb.Show();
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
                
                TaskbarItemProgressState progressBarState;
                
                if (t.ExpirationStatus == TimerStatus.Red)
                    progressBarState = TaskbarItemProgressState.Error;
                else if (t.ExpirationStatus == TimerStatus.Yellow)
                    progressBarState = TaskbarItemProgressState.Paused;
                else
                    progressBarState = TaskbarItemProgressState.Normal;
                
                this.TaskbarItemInfo = new TaskbarItemInfo
                {
                    ProgressState = progressBarState,
                    ProgressValue = 1d - ((double)t.TimeRemaining.TotalSeconds / t.Interval.TotalSeconds)
                };
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.TaskbarItemInfo = new TaskbarItemInfo();
        }

        private void Button_ActionTimerStart_Click(object sender, RoutedEventArgs e)
        {
            this.GetTimerForButton(sender).StartActionTimer();
        }

        private void Button_ActionTimerSkip_Click(object sender, RoutedEventArgs e)
        {
            this.GetTimerForButton(sender).SkipActionTimer();
        }

        private void Button_NextTimerAction_Click(object sender, RoutedEventArgs e)
        {
            this.GetTimerForButton(sender).AdvanceActivity();
        }

        private void Button_RestartTimer_Click(object sender, RoutedEventArgs e)
        {
            this._VM.RestartTimer(this.GetTimerForButton(sender));
        }

        private void Button_RestartAllTimers_Click(object sender, RoutedEventArgs e)
        {
            this._VM.RestartAllTimers();
        }

        private Timer GetTimerForButton(Object sender)
        {
            return (sender as FrameworkElement).DataContext as Timer;
        }
    }
}
