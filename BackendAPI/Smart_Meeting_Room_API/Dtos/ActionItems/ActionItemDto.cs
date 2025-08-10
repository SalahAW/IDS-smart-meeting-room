namespace Smart_Meeting_Room_API.Dtos.ActionItems
{
    public class ActionItemDto
    {
        public int Id { get; set; }
        public int MomId { get; set; }
        public string Description { get; set; } = null!;
        public int? AssignedTo { get; set; }
        public DateOnly? DueDate { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
