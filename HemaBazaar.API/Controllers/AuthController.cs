using Domain.Entities;
using HemaBazaar.API.Models;
using HemaBazaar.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HemaBazaar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly UserManager<AppUser> _userManager;
        //private readonly IConfiguration _configuration;

        //public AuthController(UserManager<AppUser> userManager, IConfiguration configuration)
        //{
        //    _userManager = userManager;
        //    _configuration = configuration;
        //}

        //public class LoginRequest
        //{
        //    public string UserName { get; set; } = string.Empty;
        //    public string Password { get; set; } = string.Empty;
        //}

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        //        return BadRequest("Username and password are required.");

        //    AppUser? user = await _userManager.FindByNameAsync(request.UserName);
        //    if (user == null)
        //        return Unauthorized("Invalid username or password.");

        //    Check lockout
        //    if (await _userManager.IsLockedOutAsync(user))
        //        return Unauthorized("Account is temporarily locked. Please try again later.");

        //    Use CheckPasswordAsync to bypass RequireConfirmedEmail restriction
        //    bool passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        //    if (!passwordValid)
        //    {
        //        await _userManager.AccessFailedAsync(user);
        //        return Unauthorized("Invalid username or password.");
        //    }

        //    Reset failed access count on successful login
        //   await _userManager.ResetAccessFailedCountAsync(user);

        //    string token = GenerateJwtToken(user);
        //    return Ok(new { token });
        //}

        //private string GenerateJwtToken(AppUser user)
        //{
        //    var jwtSettings = _configuration.GetSection("Jwt");
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(ClaimTypes.Name, user.UserName!),
        //        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    };

        //    int expireMinutes = int.TryParse(jwtSettings["ExpireMinutes"], out int mins) ? mins : 60;

        //    var token = new JwtSecurityToken(
        //        issuer: jwtSettings["Issuer"],
        //        audience: jwtSettings["Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(expireMinutes),
        //        signingCredentials: credentials
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        IJwtService _jwtService;
        UserManager<AppUser> _userManager;

        public AuthController(IJwtService jwtService, UserManager<AppUser> userManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
           AppUser user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user,model.Password))
            {
               string token = _jwtService.CreateToken(user.Id.ToString(), user.Email);

                Response.Cookies.Append("access_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddMinutes(60)
                });

                return Ok(new {token = token, ExpireDate = DateTime.Now.AddMinutes(60)});
            }
            return Unauthorized("Username or Password is not correct.");
        }

        //2 Kas?m 3:13:00dan devam.
    }
}
