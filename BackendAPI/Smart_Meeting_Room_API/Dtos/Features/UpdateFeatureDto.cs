namespace Smart_Meeting_Room_API.Dtos.Features
{
    public class UpdateFeatureDto
    {
        public string FeatureName { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; }
    }
}
