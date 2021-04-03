using System;
using System.Collections.Generic;

namespace GameDevHQ.Scripts.Payment
{
    public class PaypalExecutePaymentResponse
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
    
    public class TransactionFee
    {
        public string value;
        public string currency;
    }
    
    public class Sale
    {
        public string id;
        public DateTime create_time;
        public DateTime update_time;
        public Amount amount;
        public string payment_mode;
        public string state;
        public string protection_eligibility;
        public string protection_eligibility_type;
        public TransactionFee transaction_fee;
        public string parent_payment;
        public List<Link> links;
    }
    
    public class RelatedResource
    {
        public Sale sale;
    }
    
}