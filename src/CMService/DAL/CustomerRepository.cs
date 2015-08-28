using CMService.Models;
using Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public Customer Get(int id)
        {
            return _customerDbContext.Customers.FirstOrDefault(x => x.Id == id);
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

        public Task<int> AddAsync(Customer item)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Customer item)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Customer item)
        {
            throw new NotImplementedException();
        }
    }
}

