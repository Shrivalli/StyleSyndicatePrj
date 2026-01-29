using Microsoft.AspNetCore.Mvc;
using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Services;

namespace StyleSyndicatePrjBE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserDataService _userDataService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserDataService userDataService, ILogger<UsersController> logger)
    {
        _userDataService = userDataService;
        _logger = logger;
    }

    /// <summary>
    /// Get user profile by ID
    /// </summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(int userId)
    {
        var user = await _userDataService.GetUserDataAsync(userId);
        
        if (user == null)
        {
            return NotFound($"User {userId} not found");
        }

        return Ok(user);
    }

    /// <summary>
    /// Create or update user profile
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrUpdateUser([FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest("Email is required");
        }

        var success = await _userDataService.SaveUserDataAsync(user);
        
        if (!success)
        {
            return StatusCode(500, "Failed to save user");
        }

        return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, user);
    }
}
