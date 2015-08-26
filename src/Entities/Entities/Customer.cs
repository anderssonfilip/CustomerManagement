using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Entities
{
    public class Customer
    {
        public Customer()
        {
            CustomerUpdates = new List<CustomerUpdate>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public int HouseNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Category { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        public ICollection<CustomerUpdate> CustomerUpdates { get; set; }

        public bool IsDeleted()
        {
            return CustomerUpdates == null ? false : (CustomerUpdates.Any(u => u.Type == UpdateType.Remove.ToString()));
        }
    }
}

