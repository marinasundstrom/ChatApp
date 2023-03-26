namespace ChatApp.Services;

public interface IAccessTokenProvider
{
    Task<string?> GetAccessTokenAsync();
}
