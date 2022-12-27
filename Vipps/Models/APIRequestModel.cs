namespace Vipps.Models
{
    public class APIRequestModel
    {
        public string access_token { get; set; }
        public string url { get; set; }
        public string orderId { get; set; }
    }

    public class PaymentRequest 
    {
        public customerInfo customerInfo { get; set; }
        public merchantInfo merchantInfo { get; set; }
        public transaction transaction { get; set; }
    }

    public class customerInfo 
    {
        public string mobileNumber { get; set; }
    }
    public class merchantInfo
    {
        public string callbackPrefix { get; set; }
        public string fallBack { get; set; }
        public bool isApp { get; set; }
        public string merchantSerialNumber { get; set; }
    }
    public class transaction
    {
        public string amount { get; set; }
        public string orderId { get; set; }
        public string transactionText { get; set; }
    }
}
