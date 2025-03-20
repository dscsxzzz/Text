using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using SharedModels.Models;

namespace SharedModels.Dtos;

public class UserReadSimpleDto : ILinkToEntity<User>
{
    public Guid UserId { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public List<ChatReadSimpleDto> Chats { get; set; } = new List<ChatReadSimpleDto>();
}
