namespace KWFCaching.Abstractions.Models
{
    public class CacheTimeSettings
    {
        private TimeSpan? _timeSpan;
        public int? Seconds { get; set; }
        public int? Minutes { get; set; }
        public int? Hours { get; set; }

        public TimeSpan? GetTimeSpan()
        {
            if (_timeSpan is null)
            {
                var hours = Hours ?? 0;
                var minutes = Minutes ?? 0;
                var seconds = Seconds ?? 0;
                if (hours == 0 && minutes == 0 && seconds == 0)
                {
                    return null;
                }

                _timeSpan = new TimeSpan(hours, minutes, seconds);
            }

            return _timeSpan;
        }
    }
}
