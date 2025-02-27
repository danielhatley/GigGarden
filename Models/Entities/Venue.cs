using GigGarden.Models.Base;

namespace GigGarden.Models.Entities
{
    public class Venue : BaseAttributes
    {
        public int VenueID { get; set; }
        public string VenueName { get; set; } = "";
        public string Address { get; set; } = "";
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? Capacity { get; set; }
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string VenueType { get; set; } = "";
    }
}
