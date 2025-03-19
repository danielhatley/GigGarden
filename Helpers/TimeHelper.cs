namespace GigGarden.Helpers
{
    public static class TimeHelper
    {
        public static (long CreatedAt, int? CreatedOffset) GetCurrentTimestamp()
        {
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Converts to Unix time in seconds
            return (unixTimestamp, null); // Offset left as null for now
        }
    }
}
