namespace Smart_Meeting_Room_API.Dtos.ActionItems
{
    public class UpdateActionItemDto
    {
        public int MomId { get; set; }
        public string Description { get; set; } = null!;
        public int? AssignedTo { get; set; }
        public DateOnly? DueDate { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; }
    }
}
