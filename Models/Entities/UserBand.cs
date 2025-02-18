using GigGarden.Models.Base;

namespace GigGarden.Models.Entities
{
    public class UserBand : BaseLinkAttributes
    {
        public int UserBandId { get; set; }
        public int UserId { get; set; }
        public int BandId { get; set; }

        // The table we worked on doesn't include UpdatedAt/By/Offset, still inherit Base Attributes or do separate?
    }
}
