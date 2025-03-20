using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SharedModels.Requests;
using SharedModels.Helpers;
using System.Security.Claims;
using GenericServices;
using SharedModels.Dtos;
using Microsoft.EntityFrameworkCore;
using MainAPI.Services;
using SharedModels.ValidationModels;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICrudServicesAsync _databaseService;
        private readonly JwtHelper _jwtHelper;
        private readonly EmailService _emailService;
        private readonly CacheService _cacheService;


        public AuthController(ICrudServicesAsync databaseService, JwtHelper jwtHelper, EmailService emailService, CacheService cacheService)
        {
            _databaseService = databaseService;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _cacheService = cacheService;
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
            model.Password = hashedPassword;
            model.UserId = Guid.NewGuid();

            // Generate a 6-digit confirmation code
            var confirmationCode = new Random().Next(100000, 999999);

            // Create a unique token to store the user data and the confirmation code
            var token = Guid.NewGuid().ToString();
            var data = new ConfirmEmail()
            {
                UserData = model,
                ConfirmationCode = confirmationCode,
            };
            // Store user data temporarily in the dictionary with expiration time
            _cacheService.SetWithExpiration(token, data, TimeSpan.FromMinutes(5));

            // Send confirmation email with the confirmation code
            var confirmationLink = $"https://yourapp.com/confirm-email?token={token}&code={confirmationCode}";
            var emailBody = $"Please use the following code to confirm your account: {confirmationCode}. If you did not request this, please ignore this email.";
            await _emailService.SendEmailAsync(model.Email, "Email Confirmation", emailBody);

            return Ok(new { Token = token });
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] int confirmationCode)
        {
            var userConfirmation = _cacheService.GetFromCache<ConfirmEmail>(token);
            if (userConfirmation is not null)
            {
                if (userConfirmation.ConfirmationCode == confirmationCode)
                {
                    var userModel = userConfirmation.UserData;
                    try
                    {
                        var user = await _databaseService.CreateAndSaveAsync(userModel);
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, "An error occurred while saving the user to the database.");
                    }

                    _cacheService.RemoveFromCache(token);

                    return Ok(new { message = "User confirmed and account created successfully." });
                }
                else
                {
                    return BadRequest("Invalid confirmation code or confirmation expired.");
                }
            }

            return BadRequest("Invalid or expired token.");
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
            return Ok(new { access_token = token, userId = user.UserId });
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            var user = await _databaseService.ReadSingleAsync<UserUpdateDto>(
                x => x.Username == model.Username);

            if (user == default)
            {
                return NotFound("User not found.");
            }

            var token = Guid.NewGuid().ToString();
            var confirmationCode = new Random().Next(100000, 999999).ToString();
            var data = new ResetPassword()
            {
                UserData = user,
                ConfirmationCode = confirmationCode,
            };

            _cacheService.SetWithExpiration(token, data, TimeSpan.FromMinutes(5));

            // Send email with the confirmation code
            var emailBody = $"Your password reset code is: <b>{confirmationCode}</b>";
            await _emailService.SendEmailAsync(user.Email, "Password Reset Code", emailBody);

            return Ok(new { Token = token });
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromQuery] string token, [FromBody] VerifyResetCodeRequest model)
        {
            var userConfirmation = _cacheService.GetFromCache<ResetPassword>(token);
            if (userConfirmation is not null)
            {
                if (userConfirmation.ConfirmationCode == model.Code)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid confirmation code or confirmation expired.");
                }
            }

            return BadRequest("Invalid or expired token.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordRequest model)
        {
            var userConfirmation = _cacheService.GetFromCache<ResetPassword>(token);
            if (userConfirmation is not null)
            {
                var user = userConfirmation.UserData;
                var hashedPassword = PasswordHelper.HashPassword(model.Password);
                user.Password = hashedPassword;

                await _databaseService.UpdateAndSaveAsync(user);

                return Ok();
            }
            return BadRequest("Invalid confirmation code or confirmation expired.");
        }

    }
}
