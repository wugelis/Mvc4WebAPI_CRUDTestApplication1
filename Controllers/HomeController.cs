using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;
using Mvc4WebAPI_CRUDTestApplication1.Models;

namespace Mvc4WebAPI_CRUDTestApplication1.Controllers
{
    public class HomeController : Controller
    {
        private static HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5731/");
            return client;
        }

        private static IEnumerable<Customers> GetAll(HttpClient client)
        {
            HttpResponseMessage response = GetMyCusAPIHttpResponseMessage(client, "api/MyCusAPI");
            IEnumerable<Customers> result = response.Content.ReadAsAsync<IEnumerable<Customers>>().Result;
            return result;
        }

        private static HttpResponseMessage GetMyCusAPIHttpResponseMessage(HttpClient client, string requestUri)
        {
            HttpResponseMessage response = client.GetAsync(requestUri).Result;
            return response;
        }

        private string GenerateCustomerID()
        {
            IEnumerable<Customers> result = GetAll(GetHttpClient());
            int count = result.Count();
            count++;
            return count.ToString();
        }

        public ActionResult Index()
        {
            HttpClient client = GetHttpClient();
            IEnumerable<Customers> result = GetAll(client);
            return View(result);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Customers customer)
        {
            HttpClient client = GetHttpClient();
            customer.CustomerID = GenerateCustomerID();
            HttpResponseMessage response = client.PostAsync<Customers>("api/MyCusAPI", customer, new XmlMediaTypeFormatter()).Result;
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            HttpResponseMessage response = GetMyCusAPIHttpResponseMessage(GetHttpClient(), string.Format("api/MyCusAPI/{0}", id));
            Customers customer = response.Content.ReadAsAsync<Customers>().Result;
            return View(customer);
        }

        [HttpPost]
        public ActionResult Edit(Customers customer)
        {
            HttpClient client = GetHttpClient();
            HttpResponseMessage response = client.PutAsync<Customers>(string.Format("api/MyCusAPI/{0}", customer.CustomerID.Trim()), customer, new XmlMediaTypeFormatter()).Result;
            return RedirectToAction("Index");
        }

        public ActionResult Details(string id)
        {
            return Edit(id);
        }

        public ActionResult Delete(string id)
        {
            return Edit(id);
        }

        [HttpPost]
        public ActionResult Delete(string id, FormCollection form)
        {
            HttpClient client = GetHttpClient();
            HttpResponseMessage response = client.DeleteAsync(string.Format("api/MyCusAPI/{0}", id)).Result;
            return RedirectToAction("Index");
        }
    }
}
