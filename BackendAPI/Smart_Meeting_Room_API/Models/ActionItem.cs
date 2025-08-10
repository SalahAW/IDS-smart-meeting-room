using System;
using System.Collections.Generic;

namespace Smart_Meeting_Room_API.Models;

public partial class ActionItem
{
    public int Id { get; set; }

    public int MomId { get; set; }

    public string Description { get; set; } = null!;

    public int? AssignedTo { get; set; }

    public DateOnly? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual User? AssignedToNavigation { get; set; }

    public virtual Mom Mom { get; set; } = null!;
}
