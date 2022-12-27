using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
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

                Guid orderId = Guid.NewGuid();

                PaymentRequest objPayment = new PaymentRequest();
                objPayment.customerInfo = new customerInfo();
                objPayment.merchantInfo = new merchantInfo();
                objPayment.transaction = new transaction();

                objPayment.customerInfo.mobileNumber = "46774765";

                objPayment.merchantInfo.callbackPrefix = "https://localhost:44389/Home/ReturnURL";
                objPayment.merchantInfo.fallBack = "https://localhost:44389/Home/SuccessURL/"+ orderId.ToString();
                objPayment.merchantInfo.isApp = false;
                objPayment.merchantInfo.merchantSerialNumber = _appSettings.VippsKey.MerchantNumber;

                objPayment.transaction.amount = "49900";
                objPayment.transaction.orderId = orderId.ToString();
                objPayment.transaction.transactionText = "One pair of Vipps socks";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_appSettings.VippsURL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_Token);
                    foreach (var header in keyValuePairs)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    //POST Method
                    string paymentsReq = JsonConvert.SerializeObject(objPayment);
                    var stringContent = new StringContent(JsonConvert.SerializeObject(objPayment), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("ecomm/v2/payments/", stringContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var payments = await response.Content.ReadAsStringAsync();
                        APIRequestModel apiRequest = JsonConvert.DeserializeObject<APIRequestModel>(payments);
                        return Redirect(apiRequest.url);
                    }
                }
            }

            return View();
        }

        public IActionResult ReturnURL(string id)
        {
            return View();
        }

        public IActionResult SuccessURL(string id)
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
                    APIRequestModel getResponseData = JsonConvert.DeserializeObject<APIRequestModel>(accessKey);
                    return getResponseData.access_token;
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