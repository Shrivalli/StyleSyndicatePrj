using StyleSyndicatePrjBE.Models;

namespace StyleSyndicatePrjBE.Services;

public interface IUserDataService
{
    Task<User?> GetUserDataAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> SaveUserDataAsync(User user);
}

public class MockUserDataService : IUserDataService
{
    private readonly Dictionary<int, User> _users = new()
    {
        { 1, new User 
        { 
            Id = 1,
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Size = "L",
            Budget = 2000,
            DislikedColors = new[] { "yellow", "neon" },
            PreferredBrands = new[] { "Gucci", "Prada", "Giorgio Armani" },
            AvoidedMaterials = new[] { "polyester" },
            FitPreference = "Slim"
        }},
        { 2, new User 
        { 
            Id = 2,
            Email = "jane.smith@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Size = "S",
            Budget = 1500,
            DislikedColors = new[] { "brown" },
            PreferredBrands = new[] { "Chanel", "Louis Vuitton" },
            AvoidedMaterials = new[] { "rough fabric" },
            FitPreference = "Regular"
        }}
    };

    public Task<User?> GetUserDataAsync(int userId)
    {
        _users.TryGetValue(userId, out var user);
        return Task.FromResult(user);
    }

    public Task<bool> SaveUserDataAsync(User user)
    {
        _users[user.Id] = user;
        return Task.FromResult(true);
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return Task.FromResult(_users.Values.AsEnumerable());
    }
}
