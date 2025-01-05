using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using SharedModels.Models;

namespace SharedModels.Dtos;

public class ChatUpdateDto : ILinkToEntity<Chat>
{
    public Guid ChatId { get; set; }

    public string Name { get; set; }
}
