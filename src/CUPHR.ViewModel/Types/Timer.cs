using Schlechtums.Core.BaseClasses;
using Schlechtums.Core.Common;
using Schlechtums.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.IO;
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
                    this.ActionTimer = new Timer($"{this.Name} supplemental activity", value.Value, "Good job!");
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

        public String Icon { get; set; }
        [DALIgnore]
        public Uri IconUri { get { return new Uri(Path.Combine(Environment.CurrentDirectory, this.Icon)); } }
        [DALIgnore]
        public Uri IconUriToast
        {
            get
            {
                var extension = Path.GetExtension(this.Icon);
                var toastIcon = this.Icon.EnsureDoesNotEndWith(extension) + $"_toast{extension}";
                var toastIconFullPath = Path.Combine(Environment.CurrentDirectory, toastIcon);
                if (File.Exists(toastIconFullPath))
                    return new Uri(toastIconFullPath);
                else
                    return this.IconUri;
            }
        }
        [DALIgnore]
        public Boolean HasIcon { get { return this.Icon.IsValued(); } }

        public Boolean IsActionTimer { get; private set; }

        private DateTime _LastStart;
        private CancellationTokenSource _CTS;
        private int _CurrentMessageIteration = 0;

        public delegate void OnElapsedHandler(Timer sender, String message);
        public event OnElapsedHandler OnElapsed;

        [DALIgnore]
        public TimeSpan TimeRemaining
        {
            get
            {
                if (this.IsActionTimer)
                {
                    //it's an action timer and the buffer period hasn't expired, return the interval so the timer doesn't move
                    if (this.ActionTimeRemaining > this.Interval)
                        return this.Interval;
                    else // it's an action timer and we've passed the buffer period, return the action time remaining to actually count down
                        return this.ActionTimeRemaining;
                }
                else
                    return this.RawTimeRemaining;
            }
        }

        [DALIgnore]
        public TimerStatus ExpirationStatus
        {
            get
            {
                if (this.TimeRemaining < TimeSpan.FromMinutes(1))
                    return TimerStatus.Red;
                else if (this.TimeRemaining < TimeSpan.FromMinutes(5))
                    return TimerStatus.Yellow;
                else
                    return TimerStatus.Green;
            }
        }

        [DALIgnore]
        public DateTime NextExpirationTime {  get { return this._LastStart + this.Interval; } }

        private TimeSpan RawTimeRemaining { get { return this.Interval - (DateTime.Now - this._LastStart); } }
        private TimeSpan ActionTimeRemaining { get { return this.RawTimeRemaining + TimeSpan.FromSeconds(3); } }

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

        [DALIgnore]
        public Boolean HasMultipleActivities { get { return this.ElapsedMessages.Count > 1; } }

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

        public void AdvanceActivity()
        {
            this._CurrentMessageIteration++;
            this.RaisePropertyChanged(nameof(NextElapsedMessage));
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

        public void Restart()
        {
            this._LastStart = DateTime.Now;
            this.RaiseOnTickProperties();
        }


        private void TimerThread()
        {
            while (this._CTS != null && !this._CTS.IsCancellationRequested)
            {
                if (this.TimeRemaining < TimeSpan.FromSeconds(0))
                {
                    this.RaiseOnElapsed(this, this.NextElapsedMessage);

                    this._LastStart = DateTime.Now;
                    this.AdvanceActivity();

                    if (this.HasAction)
                    {
                        this.ActionTimerEnabled = true;
                    }
                }

                this.RaiseOnTickProperties();
                Thread.Sleep(1000);
            }
        }

        private void RaiseOnTickProperties()
        {
            this.RaisePropertyChanged(nameof(TimeRemaining));
            this.RaisePropertyChanged(nameof(ExpirationStatus));
        }

        private void RaiseOnElapsed(Timer sender, String nextElapsedMessage)
        {
            var oe = this.OnElapsed;
            if (oe != null)
                oe(sender, nextElapsedMessage);
        }
    }
}
