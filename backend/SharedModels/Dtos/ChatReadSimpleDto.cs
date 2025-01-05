using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using SharedModels.Models;

namespace SharedModels.Dtos;

public class ChatReadSimpleDto : ILinkToEntity<Chat>
{
    public Guid ChatId { get; set; }

    public string Name { get; set; }

    public Guid UserId { get; set; }

    public DateTime? CreatedAt { get; set; }
}
