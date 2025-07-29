using Microsoft.EntityFrameworkCore;

namespace IDS_smart_meeting_room_API.db_Entities
{
    [PrimaryKey("Id")]
    public class Role
    {
        public int Id { get; set; }
        public string roleName { get; set; }

    }
}
