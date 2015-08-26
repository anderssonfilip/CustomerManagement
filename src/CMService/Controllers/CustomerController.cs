using CMService.DAL;
using CMService.Search;
using Entities;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CMService.Controllers
{
    /// <summary>
    /// Contains actions to search customers as well as basic actions on a customer
    /// </summary>
    [Route("api/[controller]/")]
    public class CustomerController : Controller
    {
        private readonly IRepository<Customer> _customerRespository;

        public CustomerController(IRepository<Customer> customerRepository)
        {
            _customerRespository = customerRepository;
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            // TODO: including CustomerUpdates will throw:
            // Newtonsoft.Json.JsonSerializationException
            // Self referencing loop detected for property 'Customer' with type 'Entities.Customer'.Path 'CustomerUpdates[0]'
            // at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.CheckForCircularReference(JsonWriter writer, Object value, JsonProperty property, JsonContract contract, JsonContainerContract containerContract, JsonProperty containerProperty)

            var customer = _customerRespository.All/*.Include(c => c.CustomerUpdates)*/.FirstOrDefault(c => c.Id == id);
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

            foreach (var match in search.FindClosestMatches(customerName, _customerRespository.All.Select(i => i.Name), 5))
            {
                yield return (new KeyValuePair<int, string>(_customerRespository.All.First(c => c.Name == match).Id, match));
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
                    _customerRespository.AddAsync(customer);
                }
                else
                {
                    _customerRespository.UpdateAsync(customer);
                }
            }

            RedirectToAction("Get", customer.Id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var customer = _customerRespository.GetById(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            _customerRespository.DeleteAsync(id);

            return new HttpStatusCodeResult(204); // 201 No Content
        }
    }
}

