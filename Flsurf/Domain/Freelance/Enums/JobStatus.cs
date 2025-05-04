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
        SentToModeration, 
        Completed,
        WaitingFreelancerApproval
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum JobReaction
    {
        /// <summary>Отправить заново в черновики (Draft)</summary>
        Resubmit,
        /// <summary>Одобрить и открыть для откликов (Open)</summary>
        Approve,
        /// <summary>Пометить на удаление (soft delete)</summary>
        Delete
    }
}
