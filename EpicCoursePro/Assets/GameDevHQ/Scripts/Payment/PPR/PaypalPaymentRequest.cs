using System;
using System.Collections.Generic;

namespace GameDevHQ.Scripts.Payment.PPR
{
    [Serializable]
    public class PaypalPaymentRequest
    {
        public string intent { get; set; }
        public Payer payer { get; set; }
        public List<Transaction> transactions { get; set; }
        public string note_to_payer { get; set; }
        public RedirectUrls redirect_urls { get; set; }
    }
    
    [Serializable]
    public class Payer
    {
        public string payment_method { get; set; }
    }
    
    [Serializable]
    public class Details
    {
        public string subtotal { get; set; }
        public string tax { get; set; }
        public string shipping { get; set; }
        public string handling_fee { get; set; }
        public string shipping_discount { get; set; }
        public string insurance { get; set; }
    }

    [Serializable]
    public class Amount
    {
        public string total { get; set; }
        public string currency { get; set; }
        public Details details { get; set; }
    }

    [Serializable]
    public class PaymentOptions
    {
        public string allowed_payment_method { get; set; }
    }

    [Serializable]
    public class Item
    {
        public string name { get; set; }
        public string description { get; set; }
        public string quantity { get; set; }
        public string price { get; set; }
        public string tax { get; set; }
        public string sku { get; set; }
        public string currency { get; set; }
    }

    [Serializable]
    public class ShippingAddress
    {
        public string recipient_name { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string city { get; set; }
        public string country_code { get; set; }
        public string postal_code { get; set; }
        public string phone { get; set; }
        public string state { get; set; }
    }

    [Serializable]
    public class ItemList
    {
        public List<Item> items { get; set; }
        public ShippingAddress shipping_address { get; set; }
    }

    [Serializable]
    public class Transaction
    {
        public Amount amount { get; set; }
        public string description { get; set; }
        public string custom { get; set; }
        public string invoice_number { get; set; }
        public PaymentOptions payment_options { get; set; }
        public string soft_descriptor { get; set; }
        public ItemList item_list { get; set; }
    }

    [Serializable]
    public class RedirectUrls
    {
        public string return_url { get; set; }
        public string cancel_url { get; set; }
    }
}