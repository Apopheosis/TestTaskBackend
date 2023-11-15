using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using User_API.DTO;
using User_API.Models;
using User_API.Services;

namespace JwtWebApiTutorial.Controllers; 

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    public static User user = new();
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public AuthController(IConfiguration configuration, IUserService userService)
    {
        _configuration = configuration;
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public ActionResult<User> GetMe()
    {
        var userName = _userService.GetMyName();
        return Ok(userName);
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDTO request)
    {
        Console.WriteLine(request.Username);
        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        user.Username = request.Username;
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.Firstname = request.Firstname;
        user.Surname = request.Surname;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.url = request.url;

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserDTO request)
    {
        if (user.Username != request.Username)
        {
            return BadRequest("User not found.");
        }

        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return BadRequest("Wrong password.");
        }

        string token = CreateToken(user);

        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken);

        return Ok(token);
    }



    [HttpPost("refresh-token")]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (!user.RefreshToken.Equals(refreshToken))
            return Unauthorized("Invalid Refresh Token.");
        if (user.TokenExpires < DateTime.Now) return Unauthorized("Token expired.");

        var token = CreateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        SetRefreshToken(newRefreshToken);

        return Ok(token);
    }

    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    private void SetRefreshToken(RefreshToken newRefreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires
        };
        Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

        user.RefreshToken = newRefreshToken.Token;
        user.TokenCreated = newRefreshToken.Created;
        user.TokenExpires = newRefreshToken.Expires;
    }

    [Authorize]
    [HttpPost("update-user")]
    public async Task<ActionResult<string>> UpdateUser([FromBody]UserDTO request)
    {
        if (request.Firstname.Length == 1)
        {
            return BadRequest("Firstname length must be greater than 1!");
        }
        var updatedUser = new User()
        {
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            PasswordSalt = user.PasswordSalt,
            Firstname = request.Firstname,
            Surname = request.Surname,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            url = request.url
        };
        user = updatedUser;
        string token = CreateToken(updatedUser);

        return Ok(token);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Username),
            new(ClaimTypes.Name, user.Firstname),
            new(ClaimTypes.Surname, user.Surname),
            new("phoneNumber", user.PhoneNumber),
            new(System.Security.Claims.ClaimTypes.Email, user.Email),
            new (ClaimTypes.Uri, user.url),
            new(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}