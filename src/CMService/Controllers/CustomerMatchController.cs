using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using CMService.Settings;
using Microsoft.Framework.OptionsModel;

namespace CMService.Controllers
{
    [Route("api/[controller]/")]
    public class CustomerMatchController : Controller
    {
        private readonly ClientSetting _clientSetting;
        private readonly GraphSetting _graphSetting;

        public CustomerMatchController(IOptions<GraphSetting> graphSetting, IOptions<ClientSetting> clientSetting)
        {
            _graphSetting = graphSetting.Options;
            _clientSetting = clientSetting.Options;
        }

        [HttpGet]
        public IActionResult Index()
        {
            Context.Response.Headers["Access-Control-Allow-Origin"] = _clientSetting.URI;

            return Json("foo");
        }
    }
}
