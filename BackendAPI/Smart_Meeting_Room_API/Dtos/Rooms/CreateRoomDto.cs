namespace Smart_Meeting_Room_API.Dtos.Rooms
{
    public class CreateRoomDto
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int Capacity { get; set; }
    }
}
