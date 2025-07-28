namespace StarterApp.Services;

public interface IUserService
{
    Task<string?> GetBioAsync(int userId);
    Task<bool>    UpdateBioAsync(int userId, string bio);
}
