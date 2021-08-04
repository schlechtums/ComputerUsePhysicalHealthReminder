using CUPHR.ViewModel.Types;
using Schlechtums.Core.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace CUPHR.View.Converters
{
    public class TimerStatusToColorConverter : IValueConverter
    {
        public TimerStatusToColorConverter()
        {
            this._StatusColorsByColor = this._StatusColorsByStatus.ToReverseDictionary();
        }

        private Dictionary<TimerStatus, Brush> _StatusColorsByStatus = new Dictionary<TimerStatus, Brush>
        {
            { TimerStatus.Green, Brushes.Green },
            { TimerStatus.Yellow, Brushes.Yellow },
            { TimerStatus.Red, Brushes.Red }
        };

        private Dictionary<Brush, TimerStatus> _StatusColorsByColor;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ret = this._StatusColorsByStatus[(TimerStatus)value];

            var sr = (SolidColorBrush)ret;

            return new SolidColorBrush(Color.FromArgb(75, sr.Color.R, sr.Color.G, sr.Color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}