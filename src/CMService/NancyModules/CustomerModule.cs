using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMService.NancyModules
{
    public class CustomerModule : NancyModule
    {
        public CustomerModule()
        {
            Get["/hello"] = parameters => "Hello World";
        }
    }
}
