using Flsurf.Application.Common.Exceptions;
using Flsurf.Domain.User.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Freelance.Entities
{
    public class JobReviewEntity : BaseAuditableEntity
    {
        [ForeignKey(nameof(Reviewer))]
        [Required]
        public Guid ReviewerId { get; set; }

        [Required]
        public UserEntity Reviewer { get; set; } = null!;

        [ForeignKey(nameof(Target))]
        [Required]
        public Guid TargetId { get; set; }

        [Required]
        public UserEntity Target { get; set; } = null!;

        [ForeignKey(nameof(Job))]
        [Required]
        public Guid JobId { get; set; }

        [Required]
        public JobEntity Job { get; set; } = null!;

        [NotMapped]
        private int _rating;

        [Required]
        public int Rating
        {
            get => _rating;
            private set => _rating = value < 0 ? 0 : value > 5 ? 5 : value;
        }

        [Required]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public DateTime ReviewDate { get; set; }

        public static JobReviewEntity Create(
            Guid reviewerId,
            Guid targetId,
            JobEntity job,
            int rating,
            string comment,
            DateTime reviewDate)
        {
            if (job.Status != Enums.JobStatus.Completed)
            {
                throw new DomainException("Статус работы не заврешен");
            }

            return new JobReviewEntity
            {
                ReviewerId = reviewerId,
                TargetId = targetId,
                JobId = job.Id,
                Rating = rating,
                Comment = comment,
                ReviewDate = reviewDate
            };
        }
    }

}
