using CUPHR.ViewModel.Types.Timers;
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
        {
            this.Timers = Assembly.GetExecutingAssembly()
                                  .GetTypes()
                                  .Where(t => !t.IsAbstract && t.IsType(typeof(TimerBase)))
                                  .Select(t => Activator.CreateInstance(t) as TimerBase)
                                  .ToList();
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