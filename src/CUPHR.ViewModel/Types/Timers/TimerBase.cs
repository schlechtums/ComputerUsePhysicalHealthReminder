using Schlechtums.Core.BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CUPHR.ViewModel.Types.Timers
{
    public abstract class TimerBase : NotifyPropertyChanged, IDisposable
    {
        public TimerBase()
        {
            this.Enabled = true;
        }

        public abstract String Name { get; }
        public abstract TimeSpan Interval { get; }
        public abstract String ExpirationMessage { get; }

        private DateTime _LastStart;
        private CancellationTokenSource _CTS;

        public delegate void OnElapsedHandler(TimerBase sender, String message);
        public event OnElapsedHandler OnElapsed;

        public TimeSpan TimeRemaining { get { return this.Interval - (DateTime.Now - this._LastStart); } }

        private Boolean _Enabled;
        public Boolean Enabled
        {
            get { return this._Enabled; }
            set
            {
                if (value != this._Enabled)
                {
                    this._Enabled = value;
                    this.RaisePropertyChanged(nameof(Enabled));
                }
            }
        }

        public void Start()
        {
            this._LastStart = DateTime.Now;
            this._CTS = new CancellationTokenSource();
            new Task(this.TimerThread, this._CTS.Token).Start();
        }

        public void Stop()
        {
            if (this._CTS != null && !this._CTS.IsCancellationRequested)
            {
                this._CTS.Cancel();
                this._CTS = null;
            }
        }

        public void Dispose()
        {
            this.Stop();
        }

        private void TimerThread()
        {
            while (this._CTS != null && !this._CTS.IsCancellationRequested)
            {
                if ((DateTime.Now - this._LastStart) > this.Interval)
                {
                    var oe = this.OnElapsed;
                    if (oe != null)
                        oe(this, this.ExpirationMessage);

                    this._LastStart = DateTime.Now;
                }

                this.RaisePropertyChanged(nameof(TimeRemaining));
                Thread.Sleep(1000);
            }
        }
    }
}