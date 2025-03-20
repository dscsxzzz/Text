using System;
using GenericServices;
using SharedModels.Models;

namespace SharedModels.Dtos;

public class UserUpdateDto : ILinkToEntity<User>
{
    public Guid UserId { get; set; }  // Required to map to the existing User entity

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string Email { get; set; }
}
