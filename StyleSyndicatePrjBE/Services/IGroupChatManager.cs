using StyleSyndicatePrjBE.Models;
using StyleSyndicatePrjBE.Agents;

namespace StyleSyndicatePrjBE.Services;

/// <summary>
/// GroupChat Manager - Orchestrates multi-agent conversation (Deprecated - use IAutoGenGroupChatManager instead)
/// Coordinates between all agents to produce a final curated outfit
/// </summary>
public interface IGroupChatManager
{
    Task<WorkflowResponse> ProcessStyleRequestAsync(int userId, string userRequest);
}
