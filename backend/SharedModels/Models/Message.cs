using System;
using System.Collections.Generic;

namespace SharedModels.Models;

public class Message
{
    public Guid MessageId { get; set; }

    public Guid ChatId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string Type { get; set; } = null!;

    public virtual Chat Chat { get; set; } = null!;
}
