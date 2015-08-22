using CMClient.Models;
using CMClient.Settings;
using Entities;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CMClient.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ServiceSetting _serviceSetting;

        public CustomerController(IOptions<ServiceSetting> serviceSetting)
        {
            _serviceSetting = serviceSetting.Options;
        }

        public IActionResult Search(Search search)
        {
            search.ServiceURI = _serviceSetting.URI;

            if (!string.IsNullOrEmpty(search.Query))
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serviceSetting.URI + "customer/" + search.Query);

                using (WebResponse response = request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        search.Result = JsonConvert.DeserializeObject<List<KeyValuePair<int, string>>>(reader.ReadToEnd());
                    }
                }
            }

            return View(search);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serviceSetting.URI + "customer/" + id);

            request.ContentType = "text/json";

            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var customer = JsonConvert.DeserializeObject<Customer>(reader.ReadToEnd());

                    return View(customer);
                }
            }
        }


        [HttpPost]
        public IActionResult Add(Search search)
        {
            // Create the Customer and show the Edit page with default values

            var request = (HttpWebRequest)WebRequest.Create(_serviceSetting.URI);
            request.Method = "POST";
            request.ContentType = "text/json";

            using (var stream = request.GetRequestStream())
            {
                using (StreamWriter sw = new StreamWriter(stream))
                    sw.Write(Json(search.Query));
            }

            var response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var customer = JsonConvert.DeserializeObject<Customer>(result);
                return Redirect("/customer/edit/" + customer.Id);
            }
        }

        [HttpPost]
        public IActionResult Save(Customer customer)
        {
            var loc = Context.Response.Headers["Location"];

            WebRequest request;

            if (customer.Id == 0)
            {
                request = WebRequest.Create(_serviceSetting.URI);
                request.Method = "POST";
            }
            else
            {
                request = WebRequest.Create(_serviceSetting.URI + customer.Id);
                request.Method = "PUT";
            }

            request.ContentType = "text/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(Json(customer));
            }

            return RedirectToAction("Search");
        }

        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serviceSetting.URI + "customer/" + customer.Id);
            request.Method = "DELETE";

            using (WebResponse response = request.GetResponse())
            {

            }

            return RedirectToAction("Search");
        }
    }
}