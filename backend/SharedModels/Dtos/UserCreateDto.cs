using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GenericServices;
using SharedModels.Models;

namespace SharedModels.Dtos;

public class UserCreateDto : ILinkToEntity<User>
{
    [JsonIgnore]
    public Guid UserId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
}
