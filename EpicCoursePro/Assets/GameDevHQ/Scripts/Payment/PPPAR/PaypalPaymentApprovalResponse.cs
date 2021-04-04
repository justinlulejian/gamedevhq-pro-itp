using System;
using System.Collections.Generic;

namespace GameDevHQ.Scripts.Payment.PPPAR
{
    [Serializable]
    public class PaypalPaymentApprovalResponse
    {
        public string id { get; set; }
        public string intent { get; set; }
        public string state { get; set; }
        public string cart { get; set; }
        public Payer payer { get; set; }
        public List<Transaction> transactions { get; set; }
        public string note_to_payer { get; set; }
        public RedirectUrls redirect_urls { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public List<Link> links { get; set; }
    }
    [Serializable]
    public class ShippingAddress
    {
        public string recipient_name { get; set; }
        public string line1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
    }
    [Serializable]
    public class PayerInfo
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string payer_id { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public string country_code { get; set; }
    }
    [Serializable]
    public class Payer
    {
        public string payment_method { get; set; }
        public string status { get; set; }
        public PayerInfo payer_info { get; set; }
    }
    [Serializable]
    public class Details
    {
        public string subtotal { get; set; }
        public string tax { get; set; }
        public string shipping { get; set; }
        public string insurance { get; set; }
        public string handling_fee { get; set; }
        public string shipping_discount { get; set; }
    }
    [Serializable]
    public class Amount
    {
        public string total { get; set; }
        public string currency { get; set; }
        public Details details { get; set; }
    }
    [Serializable]
    public class Payee
    {
        public string merchant_id { get; set; }
        public string email { get; set; }
    }
    [Serializable]
    public class Item
    {
        public string name { get; set; }
        public string sku { get; set; }
        public string description { get; set; }
        public string price { get; set; }
        public string currency { get; set; }
        public int quantity { get; set; }
    }
    [Serializable]
    public class ItemList
    {
        public List<Item> items { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public string shipping_phone_number { get; set; }
    }
    [Serializable]
    public class Transaction
    {
        public Amount amount { get; set; }
        public Payee payee { get; set; }
        public string description { get; set; }
        public string custom { get; set; }
        public string invoice_number { get; set; }
        public string soft_descriptor { get; set; }
        public ItemList item_list { get; set; }
        public List<object> related_resources { get; set; }
    }
    [Serializable]
    public class RedirectUrls
    {
        public string return_url { get; set; }
        public string cancel_url { get; set; }
    }
    [Serializable]
    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }
}
