namespace Vipps.Models
{
    public class AppSettings
    {
        public string VippsURL { get; set; }
        public VippsKey VippsKey { get; set; }
    }

    public class VippsKey
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SubscriptionKey { get; set; }
        public string MerchantNumber { get; set; }
    }
}
