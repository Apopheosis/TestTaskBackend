using System.Security.Claims;
using User_API.Models;
using User_API.Services;

namespace JwtWebApiTutorial.Services.UserService;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public User GetMyName()
    {
        var user = new User();
        if (_httpContextAccessor.HttpContext != null)
        {
            user.Firstname = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            user.Surname = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Surname);
            user.url = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Uri);
            user.PhoneNumber = _httpContextAccessor.HttpContext.User.FindFirstValue("phoneNumber");
            user.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
        }

        return user;
    }
    
}