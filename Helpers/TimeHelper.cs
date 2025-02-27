namespace GigGarden.Helpers
{
    public static class TimeHelper
    {
        // Returns the current UTC time as a Unix timestamp in seconds.
        public static long GetCurrentUnixTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        // Returns the UTC offset in minutes for the current user/system.
        public static int GetCurrentUtcOffset()
        {
            return (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
        }

        // Returns a timestamp object containing time and offset
        public static (long Timestamp, int Offset) GetTimestampWithOffset()
        {
            return (GetCurrentUnixTimestamp(), GetCurrentUtcOffset());
        }
    }

}
