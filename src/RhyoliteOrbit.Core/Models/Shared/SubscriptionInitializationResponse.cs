namespace RhyoliteOrbit.Models.Shared
{
    public class SubscriptionInitializationResponse
    {
        public string PaymentMode { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string CheckoutUrl { get; set; }
        public string ClientReference { get; set; }
        public string ClientReferenceId { get; set; }
        public string HubtelPreApprovalId { get; set; }
        public string OtpPrefix { get; set; }
        public string PreapprovalStatus { get; set; }
    }
}