using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace User_API.Models;

public class User : IdentityUser
{
    [JsonProperty("name")]
    public string Username { get; set; } = string.Empty;
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
    public string? Firstname { get; set; } = string.Empty;
    [JsonProperty("surname")]
    public string? Surname { get; set; } = string.Empty;
    public override string? Email { get; set; } = string.Empty;
    public override string? PhoneNumber { get; set; } = string.Empty;
    public string? url { get; set; } = string.Empty;
}