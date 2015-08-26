using CMService.Models;
using Entities;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CMService.Migrations
{
    public class SeedCustomers
    {
        private static readonly Regex csvRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        internal static void Seed(string connectionString)
        {
            var request = WebRequest.Create("https://docs.google.com/spreadsheets/d/1UZQLqByd8AZR3wM5Cb0qmX_0eScrwfwee3iAONk3O5A/export?format=csv");

            using (var customerDbContext = new CustomerDbContext(connectionString))
            {
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

                        var customer = customerDbContext.Customers.Add(new Customer
                        {
                            Name = fields[0].Trim(' ', '"'),
                            Gender = fields[1].Trim(' ', '"'),
                            HouseNumber = int.Parse(fields[2].Trim(' ', '"')),
                            AddressLine1 = fields[3].Trim(' ', '"'),
                            State = fields[4].Trim(' ', '"'),
                            Country = fields[5].Trim(' ', '"'),
                            Category = fields[6].Trim(' ', '"').Split(' ')[1],
                            DateOfBirth = DateTime.Parse(fields[7].Trim(' ', '"'))
                        }).Entity;

                        customer.CustomerUpdates.Add(
                        new CustomerUpdate
                        {
                            Customer = customer,
                            Timestamp = DateTime.Now,
                            Type = UpdateType.Add.ToString()
                        });



                    }
                    customerDbContext.SaveChanges();

                    //foreach (var customer in customerDbContext.Customers)
                    //{

                    //}
                    //customerDbContext.SaveChanges();
                }
            }
        }
    }
}
