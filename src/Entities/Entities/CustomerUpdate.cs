using Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public enum UpdateType { Add, Remove, Update }

    public class CustomerUpdate
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; }

        public DateTime Timestamp { get; set; }

        // Relationships

        [Required]
        public virtual Customer Customer { get; set; }

    }
}

