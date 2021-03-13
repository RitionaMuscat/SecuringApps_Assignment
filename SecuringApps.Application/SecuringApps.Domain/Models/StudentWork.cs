using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecuringApps.Domain.Models
{
    public class StudentWork
    {
        [Key]
        public Guid Id { get; set; }

        public string filePath { get; set; }

        public string workOwner { get; set; }

        public DateTime submittedOn { get; set; }

        public bool isDigitallySigned { get; set; }



        public string signature { get; set; }

        [Required]
        public virtual StudentTask StudentTask { get; set; }

        [ForeignKey("StudentTask")]
        public Guid TaskId { get; set; }
    }
}
