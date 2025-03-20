using System;
using System.Collections.Generic;

namespace SharedModels.Models;

public class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();
}
