using GigGarden.Models.Base;

namespace GigGarden.Models.Entities
{
    public class UserRole : BaseLinkAttributes //Base
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }


    }
}
