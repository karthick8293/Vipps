using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using Vipps.Models;

namespace Vipps.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _appSettings;
        public HomeController(ILogger<HomeController> logger, IOptions<AppSettings> options)
        {
            _logger = logger;
            _appSettings = options.Value;
        }

        public async Task<IActionResult> Index()
        {
            string access_Token = await GetAccessId();

            if (!string.IsNullOrEmpty(access_Token))
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("Ocp-Apim-Subscription-Key", _appSettings.VippsKey.SubscriptionKey);
                keyValuePairs.Add("Merchant-Serial-Number", _appSettings.VippsKey.MerchantNumber);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_appSettings.VippsURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    foreach (var header in keyValuePairs)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_Token);
                    //POST Method
                    var department = "{}";
                    HttpResponseMessage response = await client.PostAsJsonAsync("ecomm/v2/payments/", department);
                    if (response.IsSuccessStatusCode)
                    {
                        var accessKey = await response.Content.ReadAsStringAsync();
                        dynamic accessDynamic = JsonConvert.DeserializeObject(accessKey);
                        return JsonConvert.DeserializeObject<string>(accessDynamic.access_token);

                    }
                }
            }

            return View();
        }

        public IActionResult ReturnURL(string id)
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<string> GetAccessId() 
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("client_id", _appSettings.VippsKey.ClientId);
            keyValuePairs.Add("client_secret", _appSettings.VippsKey.ClientSecret);
            keyValuePairs.Add("Ocp-Apim-Subscription-Key", _appSettings.VippsKey.SubscriptionKey);
            keyValuePairs.Add("Merchant-Serial-Number", _appSettings.VippsKey.MerchantNumber);


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_appSettings.VippsURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                foreach (var header in keyValuePairs)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                //POST Method
                var keyRequest = "{}";
                HttpResponseMessage response = await client.PostAsJsonAsync("accessToken/get", keyRequest);
                if (response.IsSuccessStatusCode)
                {
                    var accessKey = await response.Content.ReadAsStringAsync();
                    dynamic accessDynamic = JsonConvert.DeserializeObject(accessKey);
                    return JsonConvert.DeserializeObject<string>(accessDynamic.access_token);

                }
            }
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}