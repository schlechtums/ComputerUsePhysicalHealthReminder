using Schlechtums.Core.Common;
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

        [DALSQLParameterName("ActionTime")]
        public String ActionTimeCsv
        {
            get { return this.ActionTime.ToString(); }
            set
            {
                if (value.IsNullOrWhitespace())
                    this.ActionTime = null;
                else
                    this.ActionTime = TimeSpan.Parse(value);
            }
        }

        [DALIgnore]
        public override TimeSpan? ActionTime
        {
            get { return base.ActionTime; }
            set { base.ActionTime = value; }
        }
    }
}