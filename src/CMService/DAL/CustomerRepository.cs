using CMService.Models;
using CMService.Settings;
using Entities;
using Microsoft.Framework.OptionsModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMService.DAL
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly CustomerDbContext _customerDbContext;

        public CustomerRepository(IOptions<DbSetting> dbSettings)
        {
            _customerDbContext = new CustomerDbContext(dbSettings.Options.ConnectionString);
        }

        public IQueryable<Customer> All
        {
            get
            {
                return _customerDbContext.Customers;
            }
        }

        public Customer GetById(int id)
        {
            return _customerDbContext.Customers.FirstOrDefault(x => x.Id == id);
        }

        public Task<int> AddAsync(Customer customer)
        {
            _customerDbContext.Customers.Add(customer);

            _customerDbContext.CustomerUpdates.Add(new CustomerUpdate
            {
                Type = UpdateType.Add.ToString(),
                Timestamp = DateTime.Now,
                Customer = customer
            });

            return _customerDbContext.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(Customer customer)
        {
            var persistedCustomer = _customerDbContext.Customers.FirstOrDefault(x => x.Id == customer.Id);
            if (persistedCustomer != null)
            {
                persistedCustomer.AddressLine1 = customer.AddressLine1;
                persistedCustomer.Category = customer.Category;
                persistedCustomer.Country = customer.Country;
                persistedCustomer.DateOfBirth = customer.DateOfBirth;
                persistedCustomer.Gender = customer.Gender;
                persistedCustomer.Name = customer.Name;
                persistedCustomer.State = customer.State;

                _customerDbContext.CustomerUpdates.Add(new CustomerUpdate
                {
                    Type = UpdateType.Update.ToString(),
                    Timestamp = DateTime.Now,
                    Customer = persistedCustomer
                });

                return _customerDbContext.SaveChangesAsync();
            }
            return new Task<int>(delegate { return 0; });
        }

        public Task<int> DeleteAsync(int id)
        {
            var item = GetById(id);

            _customerDbContext.CustomerUpdates.Add(new CustomerUpdate
            {
                Type = UpdateType.Remove.ToString(),
                Timestamp = DateTime.Now,
                Customer = item
            });

            return _customerDbContext.SaveChangesAsync();
        }
    }
}

