using System.Threading;

namespace DotNetCoreSqlDb.Service
{
    public class Help
    {
        public static DateTime GetEstDatetime()
        {
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
        }

        public static DateTime GetEstDatetime(DateTime dateTime)
        {
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, easternZone);
        }

    }
}
