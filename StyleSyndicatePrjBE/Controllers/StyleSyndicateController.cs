using Microsoft.AspNetCore.Mvc;
using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Services;

namespace StyleSyndicatePrjBE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StyleSyndicateController : ControllerBase
{
    private readonly IGroupChatManager _groupChatManager;
    private readonly ILogger<StyleSyndicateController> _logger;

    public StyleSyndicateController(IGroupChatManager groupChatManager, ILogger<StyleSyndicateController> logger)
    {
        _groupChatManager = groupChatManager;
        _logger = logger;
    }

    /// <summary>
    /// Triggers the multi-agent fashion orchestration workflow
    /// </summary>
    /// <param name="userId">The ID of the user requesting style advice</param>
    /// <param name="request">The user's style request with occasion, location, and preferences</param>
    /// <returns>A complete workflow response with agent conversations and curated outfit</returns>
    [HttpPost("curate-outfit")]
    [ProducesResponseType(typeof(WorkflowResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CurateOutfit(int userId, [FromBody] string userRequest)
    {
        if (userId <= 0 || string.IsNullOrWhiteSpace(userRequest))
        {
            return BadRequest("Invalid user ID or request content");
        }

        try
        {
            _logger.LogInformation("Style curation requested for user {UserId}", userId);
            
            var result = await _groupChatManager.ProcessStyleRequestAsync(userId, userRequest);
            
            _logger.LogInformation("Style curation completed for user {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during style curation for user {UserId}", userId);
            return StatusCode(500, new { error = "An error occurred during style curation", details = ex.Message });
        }
    }

    [HttpGet("workflow-history/{requestId}")]
    [ProducesResponseType(typeof(WorkflowResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetWorkflowHistory(int requestId)
    {
        // In a real implementation, this would retrieve from a database
        _logger.LogInformation("Retrieving workflow history for request {RequestId}", requestId);
        
        return Ok(new WorkflowResponse 
        { 
            RequestId = requestId,
            Messages = new List<AgentMessage>(),
            Status = "Not Found"
        });
    }
}
