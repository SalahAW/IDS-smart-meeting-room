using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Smart_Meeting_Room_API.Models;

public partial class Feature
{
    public int Id { get; set; }

    public string FeatureName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
