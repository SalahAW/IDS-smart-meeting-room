namespace Smart_Meeting_Room_API.Dtos.Meetings
{
    public class UpdateMeetingDto
    {
        public int RoomId { get; set; }
        public string Title { get; set; } = null!;
        public string? Agenda { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? CreatedBy { get; set; }
        public string? Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
