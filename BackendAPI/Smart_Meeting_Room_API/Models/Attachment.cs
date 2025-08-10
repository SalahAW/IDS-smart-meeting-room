using System;
using System.Collections.Generic;

namespace Smart_Meeting_Room_API.Models;

public partial class Attachment
{
    public int Id { get; set; }

    public int MomId { get; set; }

    public string FilePath { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Mom Mom { get; set; } = null!;
}
