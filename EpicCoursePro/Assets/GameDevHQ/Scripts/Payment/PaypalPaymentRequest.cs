using System;
using System.Collections.Generic;

namespace GameDevHQ.Scripts.Payment
{
    [Serializable]
    public class PaypalPaymentRequest
    {
        public string intent;
        public Payer payer;
        public List<Transaction> transactions;
        public string note_to_payer;
        public RedirectUrls redirect_urls;
    }
    
    [Serializable]
    public class Payer
    {
        public string payment_method;
        public string status;
        public PayerInfo payer_info;
    }

    [Serializable]
    public class Details
    {
        public string subtotal;
        public string tax;
        public string shipping;
        public string handling_fee;
        public string shipping_discount;
        public string insurance;
    }

    [Serializable]
    public class Amount
    {
        public string total;
        public string currency;
        public Details details;
    }

    [Serializable]
    public class PaymentOptions
    {
        public string allowed_payment_method;
    }

    [Serializable]
    public class Item
    {
        public string name;
        public string description;
        public string quantity;
        public string price;
        public string tax;
        public string sku;
        public string currency;
    }

    [Serializable]
    public class ShippingAddress
    {
        public string recipient_name;
        public string line1;
        public string line2;
        public string city;
        public string country_code;
        public string postal_code;
        public string phone;
        public string state;
    }

    [Serializable]
    public class ItemList
    {
        public List<Item> items;
        public ShippingAddress shipping_address;
    }

    [Serializable]
    public class Transaction
    {
        public Amount amount;
        public string description;
        public string custom;
        public string invoice_number;
        public PaymentOptions payment_options;
        public string soft_descriptor;
        public ItemList item_list;
        public Payee payee;
        public List<RelatedResource> related_resources;
    }

    [Serializable]
    public class RedirectUrls
    {
        public string return_url;
        public string cancel_url;
    }

}