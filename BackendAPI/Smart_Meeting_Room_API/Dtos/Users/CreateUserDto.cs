﻿namespace Smart_Meeting_Room_API.Dtos.Users
{
    public class CreateUserDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!; 
        public int RoleId { get; set; }
    }
}
