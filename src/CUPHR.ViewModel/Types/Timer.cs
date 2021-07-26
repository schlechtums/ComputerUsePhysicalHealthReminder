using Schlechtums.Core.BaseClasses;
using Schlechtums.Core.Common;
using Schlechtums.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CUPHR.ViewModel.Types
{
    public class Timer : NotifyPropertyChanged, IDisposable
    {
        public Timer()
        {
            this.Enabled = true;
        }

        public String Name { get; set; }
        public virtual TimeSpan Interval { get; set; }
        
        [DALSQLParameterName("ElapsedMessages")]
        public String ElapsedMessagesCsv
        {
            get { return this.ElapsedMessages?.Join("|"); }
            set { this.ElapsedMessages = value.Split('|').ToList(); }
        }

        [DALIgnore]
        public List<String> ElapsedMessages { get; set; }

        public String NextElapsedMessage
        {
            get
            {
                return this.ElapsedMessages[this._CurrentMessageIteration % this.ElapsedMessages.Count];
            }
        }

        private DateTime _LastStart;
        private CancellationTokenSource _CTS;
        private int _CurrentMessageIteration = 0;

        public delegate void OnElapsedHandler(Timer sender, String message);
        public event OnElapsedHandler OnElapsed;

        [DALIgnore]
        public TimeSpan TimeRemaining { get { return this.Interval - (DateTime.Now - this._LastStart); } }

        private Boolean _Enabled;
        [DALIgnore]
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
                        oe(this, this.NextElapsedMessage);

                    this._LastStart = DateTime.Now;
                    this._CurrentMessageIteration++;
                    this.RaisePropertyChanged(nameof(NextElapsedMessage));
                }

                this.RaisePropertyChanged(nameof(TimeRemaining));
                Thread.Sleep(1000);
            }
        }
    }
}