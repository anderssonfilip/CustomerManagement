using CMService.DAL;
using CMService.Search;
using CMService.Settings;
using Entities;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMService.Controllers
{
    /// <summary>
    /// Contains actions to search customers as well as basic actions on a customer
    /// </summary>
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly CustomerDbContext _customerDbContext;

        public CustomerController(IOptions<DbSetting> dbSettings)
        {
            _customerDbContext = new CustomerDbContext(dbSettings.Options.ConnectionString);
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var customer = _customerDbContext.Customers.FirstOrDefault(c => c.Id == id);
            var json = Json(customer);

            return json;
        }

        [HttpGet("{customerName}")]
        public IEnumerable<KeyValuePair<int, string>> Search(string customerName)
        {
            if (string.IsNullOrEmpty(customerName) || string.IsNullOrWhiteSpace(customerName))
            {
                yield return new KeyValuePair<int, string>();
            }

            var search = new CustomerSearch(new LevenshteinDistance());

            foreach (var match in search.FindClosestMatches(customerName, _customerDbContext.Customers.Select(i => i.Name), 5))
            {
                yield return (new KeyValuePair<int, string>(_customerDbContext.Customers.First(c => c.Name == match).Id, match));
            }
        }

        [AcceptVerbs("POST", "PUT")]
        public void CreateOrUpdate([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                Context.Response.StatusCode = 400;
            }
            else
            {
                if (customer.Id == 0)
                {
                    customer.Updates.Add(new CustomerUpdate
                    {
                        Type = UpdateType.Add.ToString(),
                        Timestamp = DateTime.Now,
                        Customer = customer
                    });

                    _customerDbContext.Customers.Add(customer);
                    _customerDbContext.SaveChangesAsync();

                    Context.Response.StatusCode = 201;
                    Context.Response.Headers["Location"] = Url.RouteUrl("Get", new { id = customer.Id }, Request.Scheme, Request.Host.ToUriComponent());
                }
                else
                {
                    // can I do this?
                    var dbCustomer = _customerDbContext.Customers.FirstOrDefault(c => c.Id == customer.Id);
                    TryUpdateModelAsync(dbCustomer);
                    _customerDbContext.SaveChangesAsync();
                   // Update(customer.Id, customer);
                }
            }
        }


        private void Update(int id, Customer customer)
        {
            var dbCustomer = _customerDbContext.Customers.FirstOrDefault(c => c.Id == id);

            if (dbCustomer == null)
                return;

            var modified = false;

            if (dbCustomer.Name != customer.Name)
            {
                dbCustomer.Name = customer.Name;
                modified = true;
            }
            if (dbCustomer.Gender != customer.Gender)
            {
                dbCustomer.Gender = customer.Gender;
                modified = true;
            }
            if (dbCustomer.HouseNumber != customer.HouseNumber)
            {
                dbCustomer.HouseNumber = customer.HouseNumber;
                modified = true;
            }
            if (dbCustomer.AddressLine1 != customer.AddressLine1)
            {
                dbCustomer.AddressLine1 = customer.AddressLine1;
                modified = true;
            }
            if (dbCustomer.State != customer.State)
            {
                dbCustomer.State = customer.State;
                modified = true;
            }
            if (dbCustomer.Country != customer.Country)
            {
                dbCustomer.Country = customer.Country;
                modified = true;
            }
            if (dbCustomer.Category != customer.Category)
            {
                dbCustomer.Category = customer.Category;
                modified = true;
            }
            if (dbCustomer.DateOfBirth != customer.DateOfBirth)
            {
                dbCustomer.DateOfBirth = customer.DateOfBirth;
                modified = true;
            }

            if (modified)
            {
                dbCustomer.Updates.Add(new CustomerUpdate
                {
                    Type = UpdateType.Update.ToString(),
                    Timestamp = DateTime.Now,
                    Customer = dbCustomer
                });
                _customerDbContext.SaveChangesAsync();
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var customer = _customerDbContext.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            var update = new CustomerUpdate
            {
                Type = UpdateType.Remove.ToString(),
                Timestamp = DateTime.Now,
                Customer = customer
            };

            customer.Updates.Add(update);
            // _customerDbContext.CustomerUpdates.Add(update);

            _customerDbContext.SaveChangesAsync();

            return new HttpStatusCodeResult(204); // 201 No Content
        }
    }
}

