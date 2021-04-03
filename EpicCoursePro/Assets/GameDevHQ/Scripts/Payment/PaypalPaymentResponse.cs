using System;
using System.Collections.Generic;

// TODO: reconcile this with execute payment response.
namespace GameDevHQ.Scripts.Payment
{
    [Serializable]
    public class PaypalPaymentResponse
    {
        public string id;
        public DateTime create_time;
        public DateTime update_time;
        public string state;
        public string intent;
        public Payer payer;
        public List<Transaction> transactions;
        public List<Link> links;
    }
    
    public class Link
    {
        public string href;
        public string rel;
        public string method;
    }
}