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

        /// <summary>
        /// Constructor used for Action Timer
        /// </summary>
        /// <param name="name"></param>
        /// <param name="interval"></param>
        /// <param name="elapsedMessages"></param>
        private Timer(String name, TimeSpan interval, String elapsedMessages)
            : this()
        {
            this.Name = name;
            this.Interval = interval;
            this.ElapsedMessages = elapsedMessages.CreateList();
            this.IsActionTimer = true;
        }

        public String Name { get; set; }
        public virtual TimeSpan Interval { get; set; }
        protected TimeSpan? _ActionTime;
        public virtual TimeSpan? ActionTime
        {
            get { return this._ActionTime; }
            set
            {
                this._ActionTime = value;

                if (value != null)
                    this.ActionTimer = new Timer($"{this.Name} supplemental action", value.Value, "Good job!");
            }
        }

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

        public Boolean IsActionTimer { get; private set; }

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

        private Boolean _ActionTimerEnabled;
        [DALIgnore]
        public Boolean ActionTimerEnabled
        {
            get { return this._ActionTimerEnabled; }
            set
            {
                if (value != this._ActionTimerEnabled)
                {
                    this._ActionTimerEnabled = value;
                    this.RaisePropertyChanged(nameof(ActionTimerEnabled));
                    this.RaisePropertyChanged(nameof(CanStart));
                }
            }
        }

        private Boolean _ActionTimerEnabledRunning;
        [DALIgnore]
        public Boolean ActionTimerRunning
        {
            get { return this._ActionTimerEnabledRunning; }
            set
            {
                if (value != this._ActionTimerEnabledRunning)
                {
                    this._ActionTimerEnabledRunning = value;
                    this.RaisePropertyChanged(nameof(ActionTimerRunning));
                    this.RaisePropertyChanged(nameof(CanStart));
                    this.RaisePropertyChanged(nameof(CanStop));
                }
            }
        }

        [DALIgnore]
        public Boolean HasAction { get { return this.ActionTimer != null; } }

        private Timer _ActionTimer;
        [DALIgnore]

        public Timer ActionTimer
        {
            get { return this._ActionTimer; }
            set
            {
                this._ActionTimer = value;
                value.OnElapsed += (s, e) =>
                {
                    this.RaiseOnElapsed(s, e);
                    this.SkipActionTimer();
                };
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

        public void StartActionTimer()
        {
            this.ActionTimerRunning = true;
            this.ActionTimer.Start();
        }

        public void SkipActionTimer()
        {
            this.ActionTimer.Stop();
            this.ActionTimerRunning = false;
            this.ActionTimerEnabled = false;
        }

        [DALIgnore]
        public Boolean CanStart
        {
            get { return this.ActionTimerEnabled && !this.ActionTimerRunning; }
        }

        [DALIgnore]
        public Boolean CanStop
        {
            get { return this.ActionTimerRunning; }
        }


        private void TimerThread()
        {
            while (this._CTS != null && !this._CTS.IsCancellationRequested)
            {
                if ((DateTime.Now - this._LastStart) > this.Interval)
                {
                    this.RaiseOnElapsed(this, this.NextElapsedMessage);

                    this._LastStart = DateTime.Now;
                    this._CurrentMessageIteration++;
                    this.RaisePropertyChanged(nameof(NextElapsedMessage));

                    if (this.HasAction)
                    {
                        this.ActionTimerEnabled = true;
                    }
                }

                this.RaisePropertyChanged(nameof(TimeRemaining));
                Thread.Sleep(1000);
            }
        }

        private void RaiseOnElapsed(Timer sender, String nextElapsedMessage)
        {
            var oe = this.OnElapsed;
            if (oe != null)
                oe(sender, nextElapsedMessage);
        }
    }
}