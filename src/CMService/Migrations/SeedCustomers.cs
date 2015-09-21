using CMService.DAL;
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

                client.Cypher.DropUniqueConstraint("g:Gender", "g.Name").ExecuteWithoutResults();
                client.Cypher.DropUniqueConstraint("a:Address", "a.Name").ExecuteWithoutResults();
                client.Cypher.DropUniqueConstraint("s:State", "s.Name").ExecuteWithoutResults();
                client.Cypher.DropUniqueConstraint("c:Country", "c.Name").ExecuteWithoutResults();
                client.Cypher.DropUniqueConstraint("cat:Category", "cat.Name").ExecuteWithoutResults();

                client.Cypher.Match("n").OptionalMatch("(n) -[r] - ()").Delete("n, r").ExecuteWithoutResults();

                client.Cypher.CreateUniqueConstraint("g:Gender", "g.Name").ExecuteWithoutResults();
                client.Cypher.CreateUniqueConstraint("a:Address", "a.Name").ExecuteWithoutResults();
                client.Cypher.CreateUniqueConstraint("s:State", "s.Name").ExecuteWithoutResults();
                client.Cypher.CreateUniqueConstraint("c:Country", "c.Name").ExecuteWithoutResults();
                client.Cypher.CreateUniqueConstraint("cat:Category", "cat.Name").ExecuteWithoutResults();

                var customers = new List<Customer>();

                var request = WebRequest.Create(dataURL);
                using (var response = request.GetResponse())
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var line = sr.ReadLine();

                    var id = 1;
                    while (line != null)
                    {
                        line = sr.ReadLine(); // skip the header line

                        if (line == null)
                            break;

                        var fields = csvRegex.Split(line);

                        customers.Add(ReadCustomer(fields, id++));
                    }
                }

                var neo4JQueries = new Neo4JQueries(client);
                customers.ForEach(c => neo4JQueries.CreateNodes(c));
                customers.ForEach(c => neo4JQueries.CreateRelationships(c));
            }
        }

        private static Customer ReadCustomer(string[] fields, int id = 0)
        {
            return new Customer
            {
                Id = id,
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
    }
}

