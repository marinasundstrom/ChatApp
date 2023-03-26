namespace ChatApp.Services;

public interface ICurrentUserService 
{
    Task<string> GetUserIdAsync();
    Task<bool> IsInRoleAsync(string role);
}
