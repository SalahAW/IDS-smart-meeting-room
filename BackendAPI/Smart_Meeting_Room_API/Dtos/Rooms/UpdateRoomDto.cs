namespace Smart_Meeting_Room_API.Dtos.Rooms
{
    public class UpdateRoomDto
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int Capacity { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
