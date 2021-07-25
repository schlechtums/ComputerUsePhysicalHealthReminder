using System;
using System.Collections.Generic;
using System.Text;

namespace CUPHR.ViewModel.Types.Timers
{
    public class Eyes202020Timer : TimerBase
    {
        public override String Name => "Eyes 20-20-20";

        public override TimeSpan Interval => TimeSpan.FromMinutes(20);

        public override String ExpirationMessage => "Focus on something 20 feet away for 20 seconds";
    }
}