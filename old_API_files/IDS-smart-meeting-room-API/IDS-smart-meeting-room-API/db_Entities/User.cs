using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDS_smart_meeting_room_API.db_Entities
{
    [PrimaryKey("Id")]
    public class User
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public int roleId { get; set; }
        [ForeignKey("roleId")]

        public Role role { get; set; }

    }
}
