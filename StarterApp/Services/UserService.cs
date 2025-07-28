using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Data;   // <— where AppDbContext lives

namespace StarterApp.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string?> GetBioAsync(int userId)
    {
        return await _db.Users
                        .Where(u => u.Id == userId)
                        .Select(u => u.Bio)
                        .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateBioAsync(int userId, string bio)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;

        user.Bio = bio;
        await _db.SaveChangesAsync();
        return true;
    }
}
