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
            Updates = new List<CustomerUpdate>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public int HouseNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Category { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOfBirth { get; set; }

        public ICollection<CustomerUpdate> Updates { get; set; }

        //public bool IsDeleted()
        //{
        //    if (Updates == null)
        //    {
        //        return false;
        //    }
        //    return (Updates.Any(u => u.Type == UpdateType.Remove));
        //}
    }
}

