﻿using LMS.Domain.Study.Enums;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Study.Dto
{
    public class CreateStudentDto
    {
        [Required]
        public Guid PurchaseId { get; set; }
        [Required]
        public Guid InstitutionId { get; set; }
        [Required]
        public string Surname { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public GenderTypes Gender { get; set; }
        public DateTime? DateOfBirth { get; set; } 
        public int? Age { get; set; }
        [Required]
        public string Phone { get; set; } = null!; 
    }
}
