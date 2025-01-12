using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SharedModels.Requests;
using SharedModels.Helpers;
using System.Security.Claims;
using GenericServices;
using SharedModels.Dtos;
using SharedModels.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICrudServicesAsync _databaseService;
        private readonly JwtHelper _jwtHelper;

        public AuthController(ICrudServicesAsync databaseService, JwtHelper jwtHelper)
        {
            _databaseService = databaseService;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var existingUser = await _databaseService.ReadManyNoTracked<UserReadSimpleDto>()
                .Where(x => x.Username == model.Username)
                .AnyAsync();

            if (existingUser)
            {
                return Conflict("User already exists.");
            }

            var hashedPassword = PasswordHelper.HashPassword(model.Password);

            var us = new User()
            {
                Username = model.Username,
                Password = hashedPassword,
            };

            model.Password = hashedPassword;
            model.UserId = Guid.NewGuid();

            try
            {
                var user = await _databaseService.CreateAndSaveAsync(model);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(new { message = "User registered successfully."});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await _databaseService.ReadSingleAsync<UserReadSimpleDto>(x => x.Username == model.Username);
            if (user == null || !PasswordHelper.ValidatePassword(user.Password, model.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = _jwtHelper.GenerateJwtToken(user.Username, user.UserId.ToString());
            return Ok(new { access_token = token });
        }


        // Change user password
        [HttpPost("change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            var username = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Token is invalid.");
            }
            var userId =new Guid(username);
            var user = await _databaseService.ReadSingleAsync<UserReadSimpleDto>(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!PasswordHelper.ValidatePassword(user.Password, model.CurrentPassword))
            {
                return Unauthorized("Current password is incorrect.");
            }
            var hashedNewPassword = PasswordHelper.HashPassword(model.NewPassword);
            user.Password = hashedNewPassword;
            await _databaseService.UpdateAndSaveAsync(user);

            return Ok(new { message = "Password updated successfully." });
        }

        
    }
}
