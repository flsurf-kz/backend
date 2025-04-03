using System.Text.Json.Serialization;

namespace Flsurf.Domain.Freelance.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum JobStatus
    {
        Open, 
        Expired, 
        Closed, 
        Accepted, 
        InContract, 
        Draft, 
        Completed,
        WaitingFreelancerApproval
    }
}
