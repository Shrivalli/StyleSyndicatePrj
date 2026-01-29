using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Data;
using Microsoft.EntityFrameworkCore;

namespace StyleSyndicatePrjBE.Services;

public class EFUserDataService : IUserDataService
{
    private readonly StyleSyndicateDbContext _context;

    public EFUserDataService(StyleSyndicateDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserDataAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<bool> SaveUserDataAsync(User user)
    {
        try
        {
            if (user.Id == 0)
            {
                _context.Users.Add(user);
            }
            else
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    _context.Entry(existingUser).CurrentValues.SetValues(user);
                }
                else
                {
                    _context.Users.Add(user);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}
