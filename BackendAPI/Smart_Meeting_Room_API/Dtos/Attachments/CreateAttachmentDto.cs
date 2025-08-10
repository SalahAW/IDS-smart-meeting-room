namespace Smart_Meeting_Room_API.Dtos.Attachments
{
    public class CreateAttachmentDto
    {
        public int MomId { get; set; }
        public string FilePath { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}
