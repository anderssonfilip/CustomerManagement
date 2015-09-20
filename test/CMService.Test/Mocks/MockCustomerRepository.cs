using CMService.DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;

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

        public Persistence Persistence
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Add(Customer item)
        {
            throw new NotImplementedException();
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

        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Customer item)
        {
            var dbItem = _items.FirstOrDefault(c => c.Id == item.Id);
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

        public Customer Get(int id)
        {
            return _items.FirstOrDefault(c => c.Id == id);
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

        public int Update(Customer item)
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
