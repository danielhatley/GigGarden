using GigGarden.Models.Base;

namespace GigGarden.Models.Entities
{
    public class UserVenue : BaseLinkAttributes
    {
        public int UserVenueId { get; set; }
        public int UserId { get; set; }
        public int VenueId { get; set; }
        public string Role { get; set; } = "";
    }
}
