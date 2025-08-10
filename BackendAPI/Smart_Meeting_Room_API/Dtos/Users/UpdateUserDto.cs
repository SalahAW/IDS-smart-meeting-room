namespace Smart_Meeting_Room_API.Dtos.Users
{
    public class UpdateUserDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public DateTime? UpdatedAt { get; set; }  // optional, server can set this automatically
    }
}
