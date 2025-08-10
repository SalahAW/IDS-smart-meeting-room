namespace Smart_Meeting_Room_API.Dtos.Users
{
    public class ChangeUserPasswordDto
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
