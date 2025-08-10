namespace Smart_Meeting_Room_API.Dtos.Features
{
    public class FeatureDto
    {
        public int Id { get; set; }
        public string FeatureName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
