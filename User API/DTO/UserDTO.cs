namespace User_API.DTO;

public class UserDTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Firstname { get; set; } = string.Empty;
    public string? Surname { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? url { get; set; } = string.Empty;
}