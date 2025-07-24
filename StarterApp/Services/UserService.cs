using Microsoft.EntityFrameworkCore;
using StarterApp.Database;
// Add the namespace where AppDbContext is defined, for example:
using StarterApp.Database.Data;

namespace StarterApp.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) => _db = db;

    public async Task<string?> GetBioAsync(int userId) =>
        await _db.Users
                 .Where(u => u.Id == userId)
                 .Select(u => u.Bio)
                 .FirstOrDefaultAsync();

    public async Task UpdateBioAsync(int userId, string? bio)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return;

        user.Bio = bio;
        await _db.SaveChangesAsync();
    }
}
