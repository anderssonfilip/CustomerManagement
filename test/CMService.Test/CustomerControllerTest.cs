using CMService.DAL;
using Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace CMService.Test
{
    public class CustomerControllerTest
    {
        private readonly string _connectionString;
        private readonly string _serviceURI;

        public CustomerControllerTest(string connectionString, string serviceURI)
        {
            _connectionString = connectionString;
            _serviceURI = serviceURI;
        }

        public bool Get()
        {
            int id;
            string customer;
            using (var customerDbContext = new CustomerDbContext(_connectionString))
            {
                id = customerDbContext.Customers.First().Id;
                customer = JsonConvert.SerializeObject(customerDbContext.Customers.First());
            }

            var request = WebRequest.Create(_serviceURI + "customer/" + id);
            var response = request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var received = reader.ReadToEnd();
                return customer.Equals(received);
            }
        }

        public bool Search()
        {
            var request = WebRequest.Create(_serviceURI + "customer/" + "customerName");
            var response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var received = reader.ReadToEnd();
                var result = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<int, string>>>(received);

                return result.Count() == 5;
            }
        }

        public bool Post()
        {
            int id;
            using (var customerDbContext = new CustomerDbContext(_connectionString))
            {
                id = customerDbContext.Customers.Max(c => c.Id) + 1;
            }

            var request = WebRequest.Create(_serviceURI + "customer/");
            request.Method = "POST";
            request.ContentType = "text/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(new Customer
                {
                    Id = 0,
                    Name = "",
                    Gender = "",
                    HouseNumber = 1,
                    AddressLine1 = "",
                    State = "",
                    Country = "",
                    Category = "",
                    DateOfBirth = new DateTime()
                }));
            }

            request.GetResponse();

            using (var customerDbContext = new CustomerDbContext(_connectionString))
            {
                var customer = customerDbContext.Customers.FirstOrDefault(c => c.Id == id);

                var hasUpdate = customer.Updates.Last().Type == UpdateType.Add.ToString();

                customerDbContext.Customers.Remove(customer);
                customerDbContext.SaveChanges();

                return hasUpdate;
            }
        }

        public bool Put()
        {
            Customer customer;
            using (var customerDbContext = new CustomerDbContext(_connectionString))
            {
                customer = customerDbContext.Customers.First();
            }

            var name = customer.Name;
            customer.Name = new string(customer.Name.Reverse().ToArray());

            var request = WebRequest.Create(_serviceURI + "customer/");
            request.Method = "PUT";
            request.ContentType = "text/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(customer));
            }

            request.GetResponse();

            using (var customerDbContext = new CustomerDbContext(_connectionString))
            {
                customer = customerDbContext.Customers.First();

                return name.Equals(new string(customer.Name.Reverse().ToArray())) &&
                       customer.Updates.Last().Type == UpdateType.Update.ToString();
            }
        }

        public bool Delete()
        {
            int id;
            using (var customerDbContext = new CustomerDbContext(_connectionString))
            {
                var customer = customerDbContext.Customers.Add(new Customer
                {
                    Name = "",
                    Gender = "",
                    HouseNumber = 1,
                    AddressLine1 = "",
                    State = "",
                    Country = "",
                    Category = "",
                    DateOfBirth = new DateTime()
                });

                customerDbContext.SaveChanges();
                id = customer.Entity.Id;
            }

            var request = WebRequest.Create(_serviceURI + "customer/" + id);
            request.Method = "DELETE";

            request.GetResponse();

            using (var customerDbContext = new CustomerDbContext(_connectionString))
            {
                var customer = customerDbContext.Customers.FirstOrDefault(c => c.Id == id);

                var hasUpdate = customer.Updates.Last().Type == UpdateType.Remove.ToString();

                customerDbContext.Customers.Remove(customer);
                customerDbContext.SaveChanges();

                return hasUpdate;
            }
        }
    }
}
