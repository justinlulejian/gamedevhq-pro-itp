using System;
using System.Collections.Generic;

namespace GameDevHQ.Scripts.Payment
{
    public class PaypalPaymentApprovalResponse
    {
        public string id;
        public string intent;
        public string state;
        public string cart;
        public Payer payer;
        public List<Transaction> transactions;
        public string note_to_payer;
        public RedirectUrls redirect_urls;
        public DateTime create_time;
        public DateTime update_time;
        public List<Link> links;
    }

    public class PayerInfo
    {
        public string email;
        public string first_name;
        public string last_name;
        public string payer_id;
        public ShippingAddress shipping_address;
        public string country_code;
    }

    public class Payee
    {
        public string merchant_id;
        public string email;
    }
}
