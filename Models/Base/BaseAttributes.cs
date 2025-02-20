namespace GigGarden.Models.Base
{
    public class BaseAttributes
    {
        public long CreatedAt { get; set; }
        public int? CreatedOffset { get; set; }
        public int CreatedBy { get; set; }
        public long? UpdatedAt { get; set; } 
        public int? UpdatedOffset { get; set; }
        public int? UpdatedBy { get; set; }
        public long? DeletedAt { get; set; } 
        public int? DeletedOffset { get; set; }
        public int? DeletedBy { get; set; }
    }
}
