using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Smart_Meeting_Room_API.Models;

public partial class Meeting
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public string Title { get; set; } = null!;

    public string? Agenda { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Status { get; set; }


    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Mom> Moms { get; set; } = new List<Mom>();

    [JsonIgnore]
    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
