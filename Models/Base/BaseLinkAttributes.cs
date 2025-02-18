namespace GigGarden.Models.Base
{
    public class BaseLinkAttributes
    {
        // Automatically set to current Unix timestamp when a new entity is created.
        public long CreatedAt { get; set; } = GetCurrentUnixTimestamp();

        // Nullable properties for timezone offset, creation, and deletions.
        public int? CreatedOffset { get; set; }
        public int CreatedBy { get; set; }
        public long? DeletedAt { get; set; }
        public int? DeletedOffset { get; set; }
        public int? DeletedBy { get; set; }

        // Helper method to convert current UTC time to a Unix timestamp (in seconds).
        public static long GetCurrentUnixTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
