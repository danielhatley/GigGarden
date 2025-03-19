using GigGarden.Models.Base;

namespace GigGarden.Models.Entities
{

    public class User : BaseAttributes
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string GivenName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? ProfilePictureUrl { get; set; } // This and below are nullable on chatgpt's suggestion
        public string? Description { get; set; }
    }


}



