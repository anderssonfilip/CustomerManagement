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
            public uint Id { get; set; }
            public string Name { get; set; }
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

                    //return client.Cypher.Match("(p:Customer)").Return<object>("p").Results.AsQueryable();
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
            item.Id = 1 + GraphClient.Cypher.Match("(c:Customer)")
                                            .Return<int>("Max(c.Id)").Results.FirstOrDefault();

            var neo4JQueries = new Neo4JQueries(GraphClient);
            neo4JQueries.MergeNodes(item);
            neo4JQueries.CreateRelationships(item);

            return item.Id;
        }

        public Task<int> AddAsync(Customer item)
        {
            throw new NotImplementedException();
        }

        public int Delete(int id)
        {
            GraphClient.Cypher.Match(string.Format("(c: Customer{{ Id: {0} }})-[r]-()", id))
                              .Delete("c,r").ExecuteWithoutResults();

            return id;
        }

        public Task<int> DeleteAsync(Customer item)
        {
            throw new NotImplementedException();
        }

        public Customer Get(int id)
        {
            return GraphClient.Cypher.Match("(c:Customer)").Where((Customer c) => c.Id == id)
                                                           .Return(c => c.As<Customer>()).Results.FirstOrDefault();
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

                    return client.Cypher.Match("(c:Category)<-[:IS_CATEGORY]-(p:Customer)")
                                        .Return((c, p) => new { name = c.As<Node>().Name, y = p.Count() })
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

                    return client.Cypher.Match("(s:State)<-[:LIVES_IN_STATE]-(p:Customer)")
                                        .Return((s, p) => new { name = s.As<Node>().Name, y = p.Count() })
                                        .Results;
                }
            }
        }

        public int Update(Customer item)
        {
            item.Id = 1 + GraphClient.Cypher.Match("(c:Customer)")
                                            .Return<int>("Max(c.Id)").Results.FirstOrDefault();

            var neo4JQueries = new Neo4JQueries(GraphClient);
            neo4JQueries.CreateNodes(item);
            neo4JQueries.CreateRelationships(item);

            return item.Id;
        }

        public Task<int> UpdateAsync(Customer item)
        {
            throw new NotImplementedException();
        }
    }
}
