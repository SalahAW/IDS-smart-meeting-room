namespace Smart_Meeting_Room_API.Dtos.Roles
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
