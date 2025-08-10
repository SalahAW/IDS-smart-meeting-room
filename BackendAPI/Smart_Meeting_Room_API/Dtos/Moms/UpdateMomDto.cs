namespace Smart_Meeting_Room_API.Dtos.Moms
{
    public class UpdateMomDto
    {
        public int MeetingId { get; set; }
        public string? Notes { get; set; }
        public string Decisions { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; }
    }
}
