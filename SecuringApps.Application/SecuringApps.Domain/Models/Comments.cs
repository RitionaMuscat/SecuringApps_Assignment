using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecuringApps.Domain.Models
{
    public class Comments
    {
        [Key]
        public Guid Id { get; set; }

        public string comment { get; set; }

        public string writtenBy { get; set; }

        public DateTime writtenOn { get; set; }
        [Required]
        public virtual StudentWork StudentWork { get; set; }

        [ForeignKey("StudentWork")]
        public Guid WorkId { get; set; }
    }
}
