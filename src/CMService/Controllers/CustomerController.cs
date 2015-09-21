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

        [HttpGet("{id:int}", Name = "Get")]
        public IActionResult Get(int id)
        {
            // TODO: including CustomerUpdates will throw:
            // Newtonsoft.Json.JsonSerializationException
            // Self referencing loop detected for property 'Customer' with type 'Entities.Customer'.Path 'CustomerUpdates[0]'
            // at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.CheckForCircularReference(JsonWriter writer, Object value, JsonProperty property, JsonContract contract, JsonContainerContract containerContract, JsonProperty containerProperty)

            Customer customer = _customerRespository.Get(id);

            if (customer != null)
                return Json(customer);
            else
                return new HttpStatusCodeResult(204);
        }

        [HttpGet("{customerName}")]
        public IEnumerable<KeyValuePair<int, string>> Search(string customerName)
        {
            if (string.IsNullOrEmpty(customerName) || string.IsNullOrWhiteSpace(customerName))
            {
                yield return new KeyValuePair<int, string>();
            }

            var search = new CustomerSearch(new LevenshteinDistance());

            IEnumerable<KeyValuePair<int, string>> values = new List<KeyValuePair<int, string>>();
            if (_customerRespository.Persistence == Persistence.SQL)
            {
                values = _customerRespository.All.Select(i => new KeyValuePair<int, string>(i.Id, i.Name)).ToList();
            }
            else if (_customerRespository.Persistence == Persistence.Graph)
            {
                var graph = _customerRespository.GraphClient;

                values = graph.Cypher.Match("(p:Customer)").Return(p => p.As<CustomerGraph.Node>()).Results
                                     .Select(p => new KeyValuePair<int, string>((int)p.Id, p.Name));
            }

            foreach (var match in search.FindClosestMatches(customerName, values.Select(i => i.Value), 5))
            {
                yield return (new KeyValuePair<int, string>(values.First(c => c.Value == match).Key, match));
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
                var id = customer.Id == 0 ? _customerRespository.Add(customer) : _customerRespository.Update(customer);
                Context.Response.Headers["Location"] = Url.RouteUrl("Get", new { id = id }, Request.Scheme, Request.Host.ToUriComponent());
                Context.Response.StatusCode = 201;
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _customerRespository.Delete(id);

            return new HttpStatusCodeResult(204);
        }
    }
}

