using GigGarden.Models.Base;

namespace GigGarden.Models.Entities
{
    public class Band : BaseAttributes
    {
        public int BandId { get; set; }
        public string Genre { get; set; } = "";
        public string BandName { get; set; } = "";
        public int? FoundedYear { get; set; }
        public string FoundingTown { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
