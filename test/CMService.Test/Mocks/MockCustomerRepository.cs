using CMService.DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMService.Test.Mocks
{
    // TODO: Figure out how this class can be used by DI in the service during test
    public class MockCustomerRepository : IRepository<Customer>
    {
        private readonly List<Customer> _items = new List<Customer>();

        public IQueryable<Customer> All
        {
            get
            {
                return _items.AsQueryable();
            }
        }

        public Task<int> AddAsync(Customer item)
        {
            _items.Add(item);

            item.CustomerUpdates.Add(new CustomerUpdate
            {
                Type = UpdateType.Add.ToString(),
                Timestamp = DateTime.Now,
                Customer = item
            });

            return new Task<int>(delegate { return 1; });
        }

        public Customer Get(int id)
        {
            return _items.FirstOrDefault(c => c.Id == id);
        }

        public Task<int> DeleteAsync(int id)
        {
            var item = _items.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                item.CustomerUpdates.Add(new CustomerUpdate
                {
                    Type = UpdateType.Remove.ToString(),
                    Timestamp = DateTime.Now,
                    Customer = item
                });
            }

            return new Task<int>(delegate { return 1; });
        }

        public Task<int> UpdateAsync(Customer item)
        {
            var i = _items.IndexOf(_items.First(c => c.Id == item.Id));
            _items.RemoveAt(i);
            _items.Add(item);
            item.CustomerUpdates.Add(new CustomerUpdate
            {
                Type = UpdateType.Update.ToString(),
                Timestamp = DateTime.Now,
                Customer = item
            });

            return new Task<int>(delegate { return 1; });
        }

        public int Add(Customer item)
        {
            throw new NotImplementedException();
        }

        public int Update(Customer item)
        {
            throw new NotImplementedException();
        }

        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Customer item)
        {
            throw new NotImplementedException();
        }
    }
}
