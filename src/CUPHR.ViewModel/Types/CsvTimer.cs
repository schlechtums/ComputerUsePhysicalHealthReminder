using Schlechtums.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CUPHR.ViewModel.Types
{
    public class CsvTimer : Timer
    {
        [DALSQLParameterName("Interval")]
        public String IntervalCsv
        {
            get { return this.Interval.ToString(); }
            set { this.Interval = TimeSpan.Parse(value); }
        }

        [DALIgnore]
        public override TimeSpan Interval { get; set; }
    }
}