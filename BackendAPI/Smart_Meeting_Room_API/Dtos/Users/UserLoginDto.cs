namespace Smart_Meeting_Room_API.Dtos.Users
{
    public class UserLoginDto
    {
        public string Identifier { get; set; } = null!; // this can be username or email depending on what the user enters
        public string Password { get; set; } = null!;
    }
}
