using Entities;
using Neo4jClient;

namespace CMService.DAL
{
    internal class Neo4JQueries
    {
        private readonly IGraphClient _graphClient;

        internal Neo4JQueries(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        internal void CreateNodes(Customer customer)
        {
            var query = _graphClient.Cypher.Create(string.Format("(p: Customer {{ Id: {0}, Name: '{1}', HouseNumber: {2}, DateOfBirth: '{3}'}})", customer.Id, customer.Name, customer.HouseNumber, customer.DateOfBirth.ToString("yyyy-MM-dd")))
                                    .Merge(string.Format("(g: Gender {{ Name: '{0}'}})", customer.Gender))
                                    .Merge(string.Format("(a: Address {{ Name: '{0}'}})", customer.AddressLine1))
                                    .Merge(string.Format("(s: State {{ Name: '{0}'}})", customer.State))
                                    .Merge(string.Format("(c: Country {{ Name: '{0}'}})", customer.Country))
                                    .Merge(string.Format("(cat: Category {{ Name: '{0}'}})", customer.Category));

            query.ExecuteWithoutResults();
        }

        internal void MergeNodes(Customer customer)
        {
            var query = _graphClient.Cypher.Merge(string.Format("(p: Customer {{ Id: {0}, Name: '{1}', HouseNumber: {2}, DateOfBirth: '{3}'}})", customer.Id, customer.Name, customer.HouseNumber, customer.DateOfBirth.ToString("yyyy-MM-dd")))
                                    .Merge(string.Format("(g: Gender {{ Name: '{0}'}})", customer.Gender))
                                    .Merge(string.Format("(a: Address {{ Name: '{0}'}})", customer.AddressLine1))
                                    .Merge(string.Format("(s: State {{ Name: '{0}'}})", customer.State))
                                    .Merge(string.Format("(c: Country {{ Name: '{0}'}})", customer.Country))
                                    .Merge(string.Format("(cat: Category {{ Name: '{0}'}})", customer.Category));

            query.ExecuteWithoutResults();
        }



        internal void CreateRelationships(Customer customer)
        {
            var query = _graphClient.Cypher.Match(string.Format("(p: Customer {{ Id: {0}, Name: '{1}', HouseNumber: {2}, DateOfBirth: '{3}'}})", customer.Id, customer.Name, customer.HouseNumber, customer.DateOfBirth.ToString("yyyy-MM-dd")))
                                    .Match(string.Format("(g: Gender {{ Name: '{0}'}})", customer.Gender))
                                    .Match(string.Format("(a: Address {{ Name: '{0}'}})", customer.AddressLine1))
                                    .Match(string.Format("(s: State {{ Name: '{0}'}})", customer.State))
                                    .Match(string.Format("(c: Country {{ Name: '{0}'}})", customer.Country))
                                    .Match(string.Format("(cat: Category {{ Name: '{0}'}})", customer.Category))
                                    .Merge("(p)-[:IS_GENDER]->(g)")
                                    .Merge("(p)-[:LIVES_ON_ADDRESS]->(a)")
                                    .Merge("(p)-[:LIVES_IN_STATE]->(s)")
                                    .Merge("(p)-[:LIVES_IN_COUNTRY]->(c)")
                                    .Merge("(p)-[:IS_CATEGORY]->(cat)");

            query.ExecuteWithoutResults();
        }
    }
}
