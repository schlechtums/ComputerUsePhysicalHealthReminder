using System;
using System.Collections.Generic;
using System.Text;

namespace CUPHR.ViewModel.Types.TimerProviders
{
    public interface ITimersProvider
    {
        List<Timer> GetTimers();
    }
}
