using System;
using System.Collections.Generic;

namespace Smart_Meeting_Room_API.Models;

public partial class Mom
{
    public int Id { get; set; }

    public int MeetingId { get; set; }

    public string? Notes { get; set; }

    public string Decisions { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ActionItem> ActionItems { get; set; } = new List<ActionItem>();

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual Meeting Meeting { get; set; } = null!;
}
