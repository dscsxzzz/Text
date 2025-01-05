using System.Data.Entity;
using System.Security.Claims;
using GenericServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Dtos;
using SharedModels.Models;

namespace MainAPI.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : Controller
{
    private readonly ICrudServicesAsync _databaseService;

    public UserController(ICrudServicesAsync databaseService)
    {
        _databaseService = databaseService;
    }

    #region User

    [HttpGet("{userId}/data")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ProfileAsync([FromRoute] Guid userId)
    {
        var userAccessingId= User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userAccessingId))
        {
            return Unauthorized("Token is invalid.");
        }

        if (userAccessingId == userId.ToString())
        {
            return Unauthorized("You cannot access this.");
        }

        var user = await _databaseService.ReadSingleAsync<UserReadSimpleDto>(userAccessingId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(user);
    }

    #endregion

    #region Chats

    [HttpGet("{userId}/chats")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UserChatsAsync([FromRoute] Guid userId)
    {
        var userAccessingId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userAccessingId))
        {
            return Unauthorized("Token is invalid.");
        }

        if (userAccessingId == userId.ToString())
        {
            return Unauthorized("You cannot access this.");
        }

        var user = await _databaseService.ReadSingleAsync<UserReadSimpleDto>(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var chats = await _databaseService.ReadManyNoTracked<ChatReadSimpleDto>()
            .Where(x => x.UserId == userId)
            .ToListAsync();
        if (chats == null)
        {
            return NotFound("Chats not found.");
        }

        return Ok(chats);
    }

    [HttpGet("{userId}/chats/{chatId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ReadUserChatAsync([FromRoute] Guid userId, [FromRoute] Guid chatId)
    {
        var userAccessingId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userAccessingId))
        {
            return Unauthorized("Token is invalid.");
        }

        if (userAccessingId == userId.ToString())
        {
            return Unauthorized("You cannot access this.");
        }

        var user = await _databaseService.ReadSingleAsync<UserReadSimpleDto>(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var chat = await _databaseService.ReadSingleAsync<ChatReadDto>(
                x => x.UserId.ToString() == userId.ToString() &&
                x.ChatId.ToString() == chatId.ToString()
            );
        if (chat == null)
        {
            return NotFound("Chats not found.");
        }

        return Ok(chat);
    }

    [HttpPost("{userId}/chats")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateUserChatAsync([FromRoute] Guid userId, [FromBody] ChatCreateDto chatCreateDto)
    {
        var userAccessingId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userAccessingId))
        {
            return Unauthorized("Token is invalid.");
        }

        if (userAccessingId == userId.ToString())
        {
            return Unauthorized("You cannot access this.");
        }

        var user = await _databaseService.ReadSingleAsync<UserReadSimpleDto>(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var chatId = new Guid();
        chatCreateDto.ChatId = chatId;

        await _databaseService.CreateAndSaveAsync(chatCreateDto);

        return Created();
    }

    [HttpPut("{userId}/chats")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateUserChatAsync([FromRoute] Guid userId, [FromBody] ChatUpdateDto chatUpdateDto)
    {
        var userAccessingId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userAccessingId))
        {
            return Unauthorized("Token is invalid.");
        }

        if (userAccessingId == userId.ToString())
        {
            return Unauthorized("You cannot access this.");
        }

        var user = await _databaseService.ReadSingleAsync<UserReadSimpleDto>(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        await _databaseService.UpdateAndSaveAsync(chatUpdateDto);

        return NoContent();
    }

    [HttpDelete("{userId}/chats/{chatId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteUserChatAsync([FromRoute] Guid userId, [FromRoute] Guid chatId)
    {
        var userAccessingId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userAccessingId))
        {
            return Unauthorized("Token is invalid.");
        }

        if (userAccessingId == userId.ToString())
        {
            return Unauthorized("You cannot access this.");
        }

        var user = await _databaseService.ReadSingleAsync<UserReadSimpleDto>(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }


        var chat = await _databaseService.ReadSingleAsync<ChatReadSimpleDto>(chatId);
        if (chat == null)
            return NotFound("Chat not found");

        await _databaseService.DeleteAndSaveAsync<Chat>(chatId);

        return NoContent();
    }
    #endregion

    #region Messages

    [HttpPost("{userId}/chats/{chatId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> PostMessageAsync([FromRoute] Guid userId, [FromRoute] Guid chatId, [FromBody] MessageCreateDto messageCreateDto)
    {
        var userAccessingId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userAccessingId))
        {
            return Unauthorized("Token is invalid.");
        }

        if (userAccessingId == userId.ToString())
        {
            return Unauthorized("You cannot access this.");
        }

        var user = await _databaseService.ReadSingleAsync<UserReadSimpleDto>(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var chat = await _databaseService.ReadSingleAsync<ChatReadSimpleDto>(chatId);
        if (chat == null)
            return NotFound("Chat not found");

        var messageId = new Guid();
        messageCreateDto.MessageId = messageId;

        await _databaseService.CreateAndSaveAsync(messageCreateDto);

        return Created();
    }

    #endregion

}