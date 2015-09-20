using CMService.DAL;
using CMService.Settings;
using Entities;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using System.Collections.Generic;
using System.Linq;

namespace CMService.Controllers
{
    /// <summary>
    /// Contains Actions to get Statistics on the Customer dataset
    /// </summary>
    [Route("api/[controller]/")]
    public class StatsController : Controller
    {
        private readonly IRepository<Customer> _customerRespository;

        private readonly string _accessControlAllowOriginURI;

        public StatsController(IRepository<Customer> customerRepository, IOptions<ClientSetting> clientSetting)
        {
            _customerRespository = customerRepository;
            _accessControlAllowOriginURI = clientSetting.Options.URI;
        }

        [HttpGet]
        [Route("Categories")]
        public IActionResult CategoryStats()
        {
            Context.Response.Headers["Access-Control-Allow-Origin"] = _accessControlAllowOriginURI;

            var categories = new List<object>();

            if (_customerRespository.Persistence  == Persistence.SQL)
            {
                categories = QueryCategories().ToList();
            }
            else if (_customerRespository.Persistence == Persistence.Graph)
            {
                categories = _customerRespository.MatchCategories.ToList();
            }

            return Json(categories);
        }

        [HttpGet]
        [Route("Locations")]
        public IActionResult LocationStats()
        {
            Context.Response.Headers["Access-Control-Allow-Origin"] = _accessControlAllowOriginURI;

            var locations = new List<object>();

            if (_customerRespository.Persistence == Persistence.SQL)
            {
                locations = QueryLocations().ToList();
            }
            else if (_customerRespository.Persistence == Persistence.Graph)
            {
                locations = _customerRespository.MatchLocations.ToList();
            }

            return Json(locations);
        }

        private IEnumerable<object> QueryCategories()
        {
            foreach (var category in _customerRespository.All.GroupBy(c => c.Category))
            {
                yield return new { name = category.Key, y = category.Count() };
            }
        }

        private IEnumerable<object> QueryLocations()
        {
            foreach (var state in _customerRespository.All.GroupBy(c => c.State))
            {
                yield return new { name = state.Key, y = state.Count() };
            }
        }
    }
}
