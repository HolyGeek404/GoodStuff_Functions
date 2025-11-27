using System.Text.Json.Serialization;

namespace GoodStuff.Functions.Models;

public class VerificationRequest
{
    
    [JsonPropertyName("userEmail")]
    public required string Email { get; set; }
    [JsonPropertyName("key")]
    public Guid VerificationKey { get; set; }
}
