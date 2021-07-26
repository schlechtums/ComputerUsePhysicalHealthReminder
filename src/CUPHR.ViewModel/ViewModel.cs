using CUPHR.ViewModel.Types;
using CUPHR.ViewModel.Types.TimerProviders;
using Schlechtums.Core.BaseClasses;
using Schlechtums.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CUPHR.ViewModel
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
            : this(new CsvTimerProvider())
        { }

        public ViewModel(ITimersProvider timersProvider)
        {
            this.Timers = timersProvider.GetTimers();
        }

        public List<Timer> Timers { get; private set; }

        public void StartAllTimers()
        {
            foreach (var t in this.Timers)
            {
                this.StartTimer(t);
            }
        }

        public void StartTimer(Timer timer)
        {
            timer.Start();
            timer.OnElapsed += this.OnTimerElapsed;
        }

        public void StopAllTimers()
        {
            foreach (var t in this.Timers)
            {
                this.StopTimer(t);
            }
        }

        public void StopTimer(Timer timer)
        {
            timer.Stop();
            timer.OnElapsed -= this.OnTimerElapsed;
        }

        public event Timer.OnElapsedHandler OnTimerElapsed;
    }
}