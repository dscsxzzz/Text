using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SharedModels.Requests;
using SharedModels.Helpers;
using System.Security.Claims;
using DatabaseService;
using SharedModels.Models;

namespace AuthService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        public AuthController(IDatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var existingUser = await _databaseService.FindUserByUsernameAsync(model.Username);
            if (existingUser != null)
            {
                return Conflict("User already exists.");
            }

            var hashedPassword = PasswordHelper.HashPassword(model.Password);
            var newUser = new User
            {
                Username = model.Username,
                Password = hashedPassword
            };

            await _databaseService.CreateUserAsync(newUser);
            return Ok(new { message = "User registered successfully." });
        }

        // Login and generate JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await _databaseService.FindUserByUsernameAsync(model.Username);
            if (user == null || !PasswordHelper.ValidatePassword(user.Password, model.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = GenerateJwtToken(user.Username);
            return Ok(new { access_token = token });
        }

        // Retrieve user profile information
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Token is invalid.");
            }

            var user = await _databaseService.FindUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new { username = user.Username });
        }

        // Change user password
        [HttpPost("change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Token is invalid.");
            }

            var user = await _databaseService.FindUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!PasswordHelper.ValidatePassword(user.Password, model.CurrentPassword))
            {
                return Unauthorized("Current password is incorrect.");
            }

            var hashedNewPassword = PasswordHelper.HashPassword(model.NewPassword);
            await _databaseService.UpdateUserPasswordAsync(username, hashedNewPassword);

            return Ok(new { message = "Password updated successfully." });
        }

        private string GenerateJwtToken(string username)
        {
            // Ensure the secret key is loaded properly from environment variables or configuration.
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT secret key is not configured.");
            }

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            // Ensure issuer and audience are loaded properly from the configuration.
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

            if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new Exception("Issuer or Audience are not configured.");
            }

            // Create the JWT token with claims and expiration time.
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(ClaimTypes.Name, username)
                },
                expires: DateTime.Now.AddHours(1), // Token expires in 1 hour
                signingCredentials: credentials
            );

            // Return the JWT token as a string.
            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
