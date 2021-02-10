using System;
using System.ComponentModel.DataAnnotations;

namespace SecuringApps.Domain.Models
{
    public class StudentTask
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
       
        public DateTime Deadline { get; set; }

        public string DocumentOwner {get; set;}

    }
}
