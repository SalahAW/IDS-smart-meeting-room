namespace Smart_Meeting_Room_API.Dtos.Moms
{
    public class CreateMomDto
    {
        public int MeetingId { get; set; }
        public string? Notes { get; set; }
        public string Decisions { get; set; } = null!;
    }
}
