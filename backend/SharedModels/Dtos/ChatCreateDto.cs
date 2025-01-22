using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GenericServices;
using SharedModels.Models;

namespace SharedModels.Dtos;

public class ChatCreateDto : ILinkToEntity<Chat>
{
    [JsonIgnore]
    public Guid ChatId { get; set; }

    public string Name { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }
}
