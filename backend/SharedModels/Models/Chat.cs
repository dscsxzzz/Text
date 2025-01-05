using System;
using System.Collections.Generic;

namespace SharedModels.Models;

public class Chat
{
    public Guid ChatId { get; set; }

    public string Name { get; set; }

    public Guid UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User User { get; set; } = null!;
}
