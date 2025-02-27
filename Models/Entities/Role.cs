using GigGarden.Models.Base;

namespace GigGarden.Models.Entities
{
    public class Role : BaseAttributes
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public string RoleDescription { get; set; } = "";
        public int RoleLevel { get; set; }
    }
}
