namespace Smart_Meeting_Room_API.Dtos.Moms
{
    public class MomDto
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string? Notes { get; set; }
        public string Decisions { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
