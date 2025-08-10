namespace Smart_Meeting_Room_API.Dtos.Attachments
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public int MomId { get; set; }
        public string FilePath { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
