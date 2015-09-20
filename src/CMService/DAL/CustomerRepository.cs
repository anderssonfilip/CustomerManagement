using CMService.Models;
using Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Neo4jClient;

namespace CMService.DAL
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly CustomerDbContext _customerDbContext;

        public CustomerRepository(CustomerDbContext customerDbContext)
        {
            _customerDbContext = customerDbContext;
        }

        public IQueryable<Customer> All
        {
            get
            {
                return _customerDbContext.Customers;
            }
        }

        public Persistence Persistence
        {
            get
            {
                return Persistence.SQL;
            }
        }

        public int Add(Customer customer)
        {
            _customerDbContext.Customers.Add(customer);

            _customerDbContext.CustomerUpdates.Add(new CustomerUpdate
            {
                Type = UpdateType.Add.ToString(),
                Timestamp = DateTime.Now,
                Customer = customer
            });

            return _customerDbContext.SaveChanges();
        }

        public Task<int> AddAsync(Customer item)
        {
            throw new NotImplementedException();
        }

        public int Delete(int id)
        {
            var item = Get(id);

            _customerDbContext.CustomerUpdates.Add(new CustomerUpdate
            {
                Type = UpdateType.Remove.ToString(),
                Timestamp = DateTime.Now,
                Customer = item
            });

            return _customerDbContext.SaveChanges();
        }

        public Task<int> DeleteAsync(Customer item)
        {
            throw new NotImplementedException();
        }

        public Customer Get(int id)
        {
            return _customerDbContext.Customers.FirstOrDefault(x => x.Id == id);
        }

        public int Update(Customer customer)
        {
            var persistedCustomer = _customerDbContext.Customers.FirstOrDefault(x => x.Id == customer.Id);
            if (persistedCustomer != null)
            {
                persistedCustomer.AddressLine1 = customer.AddressLine1;
                persistedCustomer.Category = customer.Category;
                persistedCustomer.Country = customer.Country;
                persistedCustomer.DateOfBirth = customer.DateOfBirth;
                persistedCustomer.Gender = customer.Gender;
                persistedCustomer.HouseNumber = customer.HouseNumber;
                persistedCustomer.Name = customer.Name;
                persistedCustomer.State = customer.State;

                _customerDbContext.CustomerUpdates.Add(new CustomerUpdate
                {
                    Type = UpdateType.Update.ToString(),
                    Timestamp = DateTime.Now,
                    Customer = persistedCustomer
                });

                return _customerDbContext.SaveChanges();
            }
            return 0;
        }

        public Task<int> UpdateAsync(Customer item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> MatchCategories
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<object> MatchLocations
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IGraphClient GraphClient
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

