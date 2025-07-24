namespace StarterApp.Services;

public interface IUserService
{
    Task<string?> GetBioAsync(int userId);
    Task          UpdateBioAsync(int userId, string? bio);
}
