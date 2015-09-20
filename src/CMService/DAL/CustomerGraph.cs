using CMService.Settings;
using Entities;
using Microsoft.Framework.OptionsModel;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMService.DAL
{
    public class CustomerGraph : IRepository<Customer>
    {
        public class Node
        {
            public uint id { get; set; }
            public string name { get; set; }
        }

        private readonly string _connectionString;

        public CustomerGraph(IOptions<GraphSetting> graphSetting)
        {
            _connectionString = graphSetting.Options.ConnectionString;
        }

        public IQueryable<Customer> All
        {
            get
            {
                using (var client = new GraphClient(new Uri(_connectionString)))
                {
                    client.Connect();

                    return null;

                    //return client.Cypher.Match("(p:Person)").Return<object>("p").Results.AsQueryable();
                }
            }
        }

        public Persistence Persistence
        {
            get
            {
                return Persistence.Graph;
            }
        }

        public int Add(Customer item)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddAsync(Customer item)
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

        public Customer Get(int id)
        {
            throw new NotImplementedException();
        }

        public IGraphClient GraphClient
        {
            get
            {
                var client = new GraphClient(new Uri(_connectionString));
                client.Connect();
                return client;
            }
        }

        public IEnumerable<object> MatchCategories
        {
            get
            {
                using (var client = new GraphClient(new Uri(_connectionString)))
                {
                    client.Connect();

                    return client.Cypher.Match("(c:Category)<-[:IS_CATEGORY]-(p:Person)")
                                        .Return((c, p) => new { name = c.As<Node>().name, y = p.Count() })
                                        .Results;
                }
            }
        }

        public IEnumerable<object> MatchLocations
        {
            get
            {
                using (var client = new GraphClient(new Uri(_connectionString)))
                {
                    client.Connect();

                    return client.Cypher.Match("(s:State)<-[:LIVES_IN_STATE]-(p:Person)")
                                        .Return((s, p) => new { name = s.As<Node>().name, y = p.Count() })
                                        .Results;
                }
            }
        }

        public int Update(Customer item)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Customer item)
        {
            throw new NotImplementedException();
        }
    }
}
