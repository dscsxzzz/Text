using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using SharedModels.Models;

namespace SharedModels.Dtos;

public class UserReadSimpleSto : ILinkToEntity<User>
{
    public Guid UserId { get; set; }

    public string? Username { get; set; }

    public List<ChatReadSimpleDto> Chats { get; set; } = new List<ChatReadSimpleDto>();
}
