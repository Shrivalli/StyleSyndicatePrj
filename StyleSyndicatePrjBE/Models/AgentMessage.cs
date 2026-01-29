namespace StyleSyndicatePrjBE.Models;

public class AgentMessage
{
    public string Agent { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class WorkflowResponse
{
    public int RequestId { get; set; }
    public List<AgentMessage> Messages { get; set; } = new();
    public CuratedOutfit? FinalOutfit { get; set; }
    public string Status { get; set; } = "Completed";
}
