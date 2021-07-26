using Schlechtums.DataAccessLayer;
using Schlechtums.DataAccessLayer.Providers.CSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUPHR.ViewModel.Types.TimerProviders
{
    public class CsvTimerProvider : ITimersProvider
    {
        public List<Timer> GetTimers()
        {
            var csv = new CSVAccess("timers.csv");
            csv.HasHeaderRow = true;
            return csv.ExecuteRead<CsvTimer>()
                      .OfType<Timer>()
                      .ToList();
        }
    }
}