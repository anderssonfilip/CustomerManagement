using CMService.DAL;
using CMService.Settings;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using System.Collections.Generic;
using System.Linq;

namespace CMService.Controllers
{
    /// <summary>
    /// Contains Actions to get Statistics on the Customer dataset
    /// </summary>
    public class StatsController : Controller
    {
        private readonly CustomerDbContext _customerDbContext;

        public StatsController(IOptions<DbSetting> dbSettings)
        {
            _customerDbContext = new CustomerDbContext(dbSettings.Options.ConnectionString);
        }

        [HttpGet]
        public IActionResult CategoryStats()
        {
            var categories = QueryCategories().ToArray();
            return Json(categories);
        }

        [HttpGet]
        public IActionResult LocationStats()
        {
            var locations = QueryLocations().ToArray();
            return Json(locations);
        }

        private IEnumerable<object> QueryCategories()
        {
            foreach (var category in _customerDbContext.Customers.GroupBy(c => c.Category))
            {
                yield return new { name = category.Key, y = category.Count() };
            }
        }

        private IEnumerable<object> QueryLocations()
        {
            foreach (var state in _customerDbContext.Customers.GroupBy(c => c.State))
            {
                yield return new { name = state.Key, y = state.Count() };
            }
        }
    }
}
