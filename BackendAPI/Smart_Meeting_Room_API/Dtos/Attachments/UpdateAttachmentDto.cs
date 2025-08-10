namespace Smart_Meeting_Room_API.Dtos.Attachments
{
    public class UpdateAttachmentDto
    {
        public int MomId { get; set; }
        public string FilePath { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; }
    }
}
