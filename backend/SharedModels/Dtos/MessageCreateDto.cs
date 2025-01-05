using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GenericServices;
using SharedModels.Models;

namespace SharedModels.Dtos;

public class MessageCreateDto : ILinkToEntity<Message>
{
    [JsonIgnore]
    public Guid MessageId { get; set; }

    [JsonIgnore]
    public Guid ChatId { get; set; }

    public string MessageText { get; set; } = null!;
}
