using Entities;
using System;


namespace Entities
{
    public enum UpdateType { Add, Remove, Update }

    public class CustomerUpdate
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public DateTime Timestamp { get; set; }

        // Relationships

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

    }
}

