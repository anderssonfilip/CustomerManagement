﻿using CMClient.Models;
using CMClient.Settings;
using Entities;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using Newtonsoft.Json;
using System;
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
                var request = WebRequest.Create(_serviceSetting.URI + "customer/" + search.Query);

                using (var response = request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    search.Result = JsonConvert.DeserializeObject<List<KeyValuePair<int, string>>>(reader.ReadToEnd());
                }
            }
            return View(search);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serviceSetting.URI + "customer/" + id);

            request.ContentType = "text/json";

            using (var response = request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var customer = JsonConvert.DeserializeObject<Customer>(reader.ReadToEnd());

                return View(customer);
            }
        }

        [HttpPost]
        public IActionResult Add(Search search)
        {
            // Create the Customer and show the Edit page with default values

            var request = WebRequest.Create(_serviceSetting.URI + "customer/");
            request.Method = "POST";
            request.ContentType = "text/json";

            using (var stream = request.GetRequestStream())
            {
                var customer = new Customer
                {
                    Name = search.Query,
                    Gender = "",
                    HouseNumber = 1,
                    AddressLine1 = "",
                    State = "",
                    Country = "",
                    Category = "",
                    DateOfBirth = new DateTime()
                };

                using (StreamWriter sw = new StreamWriter(stream))
                    sw.Write(JsonConvert.SerializeObject(customer));
            }

            using (var response = request.GetResponse())
            {
                var loc = response.Headers["Location"];
                return RedirectToAction("Edit", new { id = loc.Substring(loc.LastIndexOf('/') + 1) });
            }
        }

        [HttpPost]
        public IActionResult Save(Customer customer)
        {
            if (customer.Id == 0)
            {
                var referer = Context.Request.Headers["Referer"];
                customer.Id = int.Parse(referer.Substring(referer.LastIndexOf('/') + 1));
            }

            var request = WebRequest.Create(_serviceSetting.URI + "customer/");
            request.ContentType = "text/json";
            request.Method = "PUT";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(customer));
            }

            using (var response = request.GetResponse())
            {
                var loc = response.Headers["Location"];
                return RedirectToAction("Edit", new { id = loc.Substring(loc.LastIndexOf('/') + 1) });
            }
        }

        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            if (customer.Id == 0)
            {
                var referer = Context.Request.Headers["Referer"];
                customer.Id = int.Parse(referer.Substring(referer.LastIndexOf('/') + 1));
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serviceSetting.URI + "customer/" + customer.Id);
            request.Method = "DELETE";

            using (var response = request.GetResponse())
            {

            }

            return RedirectToAction("Search");
        }
    }
}