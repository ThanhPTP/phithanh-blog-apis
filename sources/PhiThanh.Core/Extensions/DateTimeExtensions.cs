namespace PhiThanh.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime BeginOfWeek(this DateTime dt)
        {
            DateTime ndt = dt.Subtract(TimeSpan.FromDays((int)dt.DayOfWeek));
            return new DateTime(ndt.Year, ndt.Month, ndt.Day, 0, 0, 0, 0);
        }

        public static DateTime EndOfWeek(this DateTime dt)
        {
            DateTime ndt = dt.BeginOfWeek().AddDays(6);
            return new DateTime(ndt.Year, ndt.Month, ndt.Day, 23, 59, 59, 999);
        }

        public static DateTime BeginOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }

        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime BeginOfMonth(this DateTime value)
        {
            return value.Date.AddDays(1 - value.Day);
        }

        public static DateTime EndOfMonth(this DateTime value)
        {
            return value.BeginOfMonth().AddMonths(1).AddDays(-1);
        }
    }
}
