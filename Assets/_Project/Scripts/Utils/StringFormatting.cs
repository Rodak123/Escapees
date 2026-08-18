using UnityEngine;

namespace GameJam
{
    public static class StringFormatting
    {
        public const int MinuteSeconds = 60;
        public const int HourSeconds = 60 * MinuteSeconds;
        public const int DaySeconds = 24 * HourSeconds;
        public const int MonthSeconds = 30 * DaySeconds;

        public static string FormatTime(float secondsTotal)
        {
            float remainingSeconds = secondsTotal;

            float monthsVal = Mathf.Floor(remainingSeconds / MonthSeconds);
            remainingSeconds -= monthsVal * MonthSeconds;

            float daysVal = Mathf.Floor(remainingSeconds / DaySeconds);
            remainingSeconds -= daysVal * DaySeconds;

            float hoursVal = Mathf.Floor(remainingSeconds / HourSeconds);
            remainingSeconds -= hoursVal * HourSeconds;

            float minutesVal = Mathf.Floor(remainingSeconds / MinuteSeconds);
            remainingSeconds -= minutesVal * MinuteSeconds;

            float secondsVal = Mathf.Floor(remainingSeconds);

            int months = Mathf.FloorToInt(monthsVal);
            int days = Mathf.FloorToInt(daysVal);
            int hours = Mathf.FloorToInt(hoursVal);
            int minutes = Mathf.FloorToInt(minutesVal);
            int seconds = Mathf.FloorToInt(secondsVal);

            if (days > 0)
            {
                return $"{months}M {days}D";
            }
            else
            {
                return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            }
        }

        public static string WrapInColor(this string text, Color? color)
        {
            if (color.HasValue) return text.WrapInColor(color.Value);
            return text;
        }

        public static string WrapInColor(this string text, Color color)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{hex}>{text}</color>";
        }
    }
}
