using System;

namespace KinderChat.Converters
{
    public class TimestampConverter : ValueConverter<DateTime, string>
    {
        public override string Convert(DateTime value)
        {
            var localTime = value.ToLocalTime();
            var dt = DateTime.Now - localTime;

            switch (dt.Days)
            {
                case 0:
                    return localTime.ToString("t");

                case 1:
                    return "YESTERDAY";

                default:
                    return localTime.ToString("MMM dd");
            }
        }
    }
}
