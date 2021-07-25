using CUPHR.ViewModel.Types.Timers;
using Schlechtums.Core.BaseClasses;
using System;
using System.Collections.Generic;

namespace CUPHR.ViewModel
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            this.Timers = new List<TimerBase>
            {
                new Eyes202020Timer()
            };
        }

        public List<TimerBase> Timers { get; private set; }

        public void StartAllTimers()
        {
            foreach (var t in this.Timers)
            {
                this.StartTimer(t);
            }
        }

        public void StartTimer(TimerBase timer)
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

        public void StopTimer(TimerBase timer)
        {
            timer.Stop();
            timer.OnElapsed -= this.OnTimerElapsed;
        }

        public event TimerBase.OnElapsedHandler OnTimerElapsed;
    }
}