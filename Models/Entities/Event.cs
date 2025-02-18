using GigGarden.Models.Base;

namespace GigGarden.Models.Entities
{
    public class Event : BaseAttributes
    {
        public int EventId { get; set; }
        public int VenueId { get; set; }
        public long EventDate { get; set; }
        public int TicketsAvailable { get; set; }
        public string EventType { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
