using CMService.Models;
using Entities;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CMService.Migrations
{
    public class SeedCustomers
    {
        private const string dataURL = "https://docs.google.com/spreadsheets/d/1UZQLqByd8AZR3wM5Cb0qmX_0eScrwfwee3iAONk3O5A/export?format=csv";
        private static readonly Regex csvRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        internal static void SeedToMSSQL(string connectionString)
        {
            using (var customerDbContext = new CustomerDbContext(connectionString))
            {
                customerDbContext.CustomerUpdates.RemoveRange(customerDbContext.CustomerUpdates);
                customerDbContext.SaveChanges();

                customerDbContext.Customers.RemoveRange(customerDbContext.Customers);
                customerDbContext.SaveChanges();

                var request = WebRequest.Create(dataURL);
                using (var response = request.GetResponse())
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var line = sr.ReadLine();

                    while (line != null)
                    {
                        line = sr.ReadLine(); // skip the header line

                        if (line == null)
                            break;

                        var fields = csvRegex.Split(line);

                        var customer = customerDbContext.Customers.Add(ReadCustomer(fields)).Entity;

                        customer.CustomerUpdates.Add(
                        new CustomerUpdate
                        {
                            Customer = customer,
                            Timestamp = DateTime.Now,
                            Type = UpdateType.Add.ToString()
                        });
                    }
                    customerDbContext.SaveChanges();
                }
            }
        }

        internal static void SeedToNeo4j(string connectionString)
        {
            using (var client = new GraphClient(new Uri(connectionString)))
            {
                client.Connect();

                client.Cypher.DropUniqueConstraint("g:Gender", "g.name").ExecuteWithoutResults();
                client.Cypher.DropUniqueConstraint("a:Address", "a.name").ExecuteWithoutResults();
                client.Cypher.DropUniqueConstraint("s:State", "s.name").ExecuteWithoutResults();
                client.Cypher.DropUniqueConstraint("c:Country", "c.name").ExecuteWithoutResults();
                client.Cypher.DropUniqueConstraint("cat:Category", "cat.name").ExecuteWithoutResults();

                client.Cypher.Match("n").OptionalMatch("(n) -[r] - ()").Delete("n, r").ExecuteWithoutResults();

                client.Cypher.CreateUniqueConstraint("g:Gender", "g.name").ExecuteWithoutResults();
                client.Cypher.CreateUniqueConstraint("a:Address", "a.name").ExecuteWithoutResults();
                client.Cypher.CreateUniqueConstraint("s:State", "s.name").ExecuteWithoutResults();
                client.Cypher.CreateUniqueConstraint("c:Country", "c.name").ExecuteWithoutResults();
                client.Cypher.CreateUniqueConstraint("cat:Category", "cat.name").ExecuteWithoutResults();

                var customers = new List<Customer>();

                var request = WebRequest.Create(dataURL);
                using (var response = request.GetResponse())
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var line = sr.ReadLine();

                    while (line != null)
                    {
                        line = sr.ReadLine(); // skip the header line

                        if (line == null)
                            break;

                        var fields = csvRegex.Split(line);

                        customers.Add(ReadCustomer(fields));
                    }
                }

                customers.ForEach(c => CreateNodes(client, c));
                customers.ForEach(c => CreateRelationships(client,c));
            }
        }

        private static Customer ReadCustomer(string[] fields)
        {
            return new Customer
            {
                Name = fields[0].Trim(' ', '"').Replace("'", "\\'"),
                Gender = fields[1].Trim(' ', '"'),
                HouseNumber = int.Parse(fields[2].Trim(' ', '"')),
                AddressLine1 = fields[3].Trim(' ', '"'),
                State = fields[4].Trim(' ', '"'),
                Country = fields[5].Trim(' ', '"'),
                Category = fields[6].Trim(' ', '"').Split(' ')[1],
                DateOfBirth = DateTime.Parse(fields[7].Trim(' ', '"'))
            };
        }

        private static void CreateNodes(GraphClient client, Customer customer)
        {
            var query = client.Cypher.Create(string.Format("(p: Person {{ name: '{0}', HouseNumber: '{1}', DateOfBirth: '{2}'}})", customer.Name, customer.HouseNumber, customer.DateOfBirth.ToString("yyyy-MM-dd")))
                                     .Merge(string.Format("(g: Gender {{ name: '{0}'}})", customer.Gender))
                                     .Merge(string.Format("(a: Address {{ name: '{0}'}})", customer.AddressLine1))
                                     .Merge(string.Format("(s: State {{ name: '{0}'}})", customer.State))
                                     .Merge(string.Format("(c: Country {{ name: '{0}'}})", customer.Country))
                                     .Merge(string.Format("(cat: Category {{ name: '{0}'}})", customer.Category));

            query.ExecuteWithoutResults();
        }

        private static void CreateRelationships(GraphClient client, Customer customer)
        {
            var query = client.Cypher.Match(string.Format("(p: Person {{ name: '{0}', HouseNumber: '{1}', DateOfBirth: '{2}'}})", customer.Name, customer.HouseNumber, customer.DateOfBirth.ToString("yyyy-MM-dd")))
                                     .Match(string.Format("(g: Gender {{ name: '{0}'}})", customer.Gender))
                                     .Match(string.Format("(a: Address {{ name: '{0}'}})", customer.AddressLine1))
                                     .Match(string.Format("(s: State {{ name: '{0}'}})", customer.State))
                                     .Match(string.Format("(c: Country {{ name: '{0}'}})", customer.Country))
                                     .Match(string.Format("(cat: Category {{ name: '{0}'}})", customer.Category))
                                     .Merge("(p)-[:IS_GENDER]->(g)")
                                     .Merge("(p)-[:LIVES_ON_ADDRESS]->(a)")
                                     .Merge("(p)-[:LIVES_IN_STATE]->(s)")
                                     .Merge("(p)-[:LIVES_IN_COUNTRY]->(c)")
                                     .Merge("(p)-[:IS_CATEGORY]->(cat)");

            query.ExecuteWithoutResults();
        }
    }
}

