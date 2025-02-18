namespace GigGarden.Models.Base
{
    public class BaseAttributes
    {

        // Automatically set to current Unix timestamp when a new entity is created.
        public long CreatedAt { get; set; } = GetCurrentUnixTimestamp();

        // Nullable properties for timezone offset, updates, and deletions.
        public int? CreatedOffset { get; set; }
        public int CreatedBy { get; set; }
        public long? UpdatedAt { get; set; } // = GetCurrentUnixTimestamp();    ?
        public int? UpdatedOffset { get; set; }
        public int? UpdatedBy { get; set; }
        public long? DeletedAt { get; set; } // = GetCurrentUnixTimestamp();    ?
        public int? DeletedOffset { get; set; }
        public int? DeletedBy { get; set; }

        // Helper method to convert current UTC time to a Unix timestamp (in seconds) - ***create it's own helper file?
        public static long GetCurrentUnixTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }


    }
}
