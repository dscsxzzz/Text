using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GenericServices;
using SharedModels.Models;

namespace SharedModels.Dtos;

public class MessageReadDto : ILinkToEntity<Message>
{
    public string MessageText { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
