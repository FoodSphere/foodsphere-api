namespace FoodSphere.Pos.Api.DTO
{
    public record StripeWebhookRequest
    {
        public string id { get; set; } = default!;
        public string @object { get; set; } = default!;
        // public string api_version { get; set; } = default!;
        // public int created { get; set; } = default!;
        public bool livemode { get; set; } = default!;
        public int pending_webhooks { get; set; } = default!;
        public string type { get; set; } = default!;
        // public StripeWebhook._Request request { get; set; } = default!;
        public StripeWebhook._Data data { get; set; } = default!;
    }
}

namespace FoodSphere.Pos.Api.DTO.StripeWebhook
{
    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    // public class _Request
    // {
    //     public object id { get; set; }
    //     public object idempotency_key { get; set; }
    // }

    public class _Data
    {
        public _Object @object { get; set; }
    }

    public class _Object
    {
        public string id { get; set; }
        public string @object { get; set; }
        public _AdaptivePricing adaptive_pricing { get; set; }
        // public object after_expiration { get; set; }
        // public object allow_promotion_codes { get; set; }
        public int amount_subtotal { get; set; }
        public int amount_total { get; set; }
        public _AutomaticTax automatic_tax { get; set; }
        // public object billing_address_collection { get; set; }
        // public _BrandingSettings branding_settings { get; set; }
        public string cancel_url { get; set; }
        // public object client_reference_id { get; set; }
        // public object client_secret { get; set; }
        // public object collected_information { get; set; }
        // public object consent { get; set; }
        // public object consent_collection { get; set; }
        public int created { get; set; }
        public string currency { get; set; }
        // public object currency_conversion { get; set; }
        // public List<object> custom_fields { get; set; }
        // public _CustomText custom_text { get; set; }
        // public object customer { get; set; }
        // public object customer_account { get; set; }
        public string customer_creation { get; set; }
        public _CustomerDetails customer_details { get; set; }
        // public object customer_email { get; set; }
        public List<object> discounts { get; set; }
        public int expires_at { get; set; }
        // public object integration_identifier { get; set; }
        // public object invoice { get; set; }
        public _InvoiceCreation invoice_creation { get; set; }
        public bool livemode { get; set; }
        // public object locale { get; set; }
        public _Metadata metadata { get; set; }
        public string mode { get; set; }
        // public object origin_context { get; set; }
        public string payment_intent { get; set; }
        // public object payment_link { get; set; }
        public string payment_method_collection { get; set; }
        // public object payment_method_configuration_details { get; set; }
        public _PaymentMethodOptions payment_method_options { get; set; }
        public List<string> payment_method_types { get; set; }
        public string payment_status { get; set; }
        // public object permissions { get; set; }
        public _PhoneNumberCollection phone_number_collection { get; set; }
        // public object recovered_from { get; set; }
        // public object saved_payment_method_options { get; set; }
        // public object setup_intent { get; set; }
        // public object shipping_address_collection { get; set; }
        // public object shipping_cost { get; set; }
        // public List<object> shipping_options { get; set; }
        public string status { get; set; }
        // public object submit_type { get; set; }
        // public object subscription { get; set; }
        public string success_url { get; set; }
        // public _TotalDetails total_details { get; set; }
        public string ui_mode { get; set; }
        // public object url { get; set; }
        // public object wallet_options { get; set; }
    }

    public class _AdaptivePricing
    {
        public bool enabled { get; set; }
    }

    // public class _Address
    // {
    //     public object city { get; set; }
    //     public object country { get; set; }
    //     public object line1 { get; set; }
    //     public object line2 { get; set; }
    //     public object postal_code { get; set; }
    //     public object state { get; set; }
    // }

    public class _AutomaticTax
    {
        public bool enabled { get; set; }
        // public object liability { get; set; }
        // public object provider { get; set; }
        // public object status { get; set; }
    }

    // public class _BrandingSettings
    // {
    //     public string background_color { get; set; }
    //     public string border_style { get; set; }
    //     public string button_color { get; set; }
    //     public string display_name { get; set; }
    //     public string font_family { get; set; }
    //     public object icon { get; set; }
    //     public object logo { get; set; }
    // }

    public class _CustomerDetails
    {
        // public _Address address { get; set; }
        // public object business_name { get; set; }
        public string email { get; set; }
        // public object individual_name { get; set; }
        // public object name { get; set; }
        // public object phone { get; set; }
        // public string tax_exempt { get; set; }
        // public List<object> tax_ids { get; set; }
    }

    // public class _CustomText
    // {
    //     public object after_submit { get; set; }
    //     public object shipping_address { get; set; }
    //     public object submit { get; set; }
    //     public object terms_of_service_acceptance { get; set; }
    // }

    public class _InvoiceCreation
    {
        public bool enabled { get; set; }
        // public _InvoiceData invoice_data { get; set; }
    }

    // public class _InvoiceData
    // {
    //     public object account_tax_ids { get; set; }
    //     public object custom_fields { get; set; }
    //     public object description { get; set; }
    //     public object footer { get; set; }
    //     public object issuer { get; set; }
    //     public _Metadata metadata { get; set; }
    //     public object rendering_options { get; set; }
    // }

    public class _Metadata
    {
        public string bill_id { get; set; }
    }

    public class _PaymentMethodOptions
    {
    }

    public class _PhoneNumberCollection
    {
        public bool enabled { get; set; }
    }

    // public class _TotalDetails
    // {
    //     public int amount_discount { get; set; }
    //     public int amount_shipping { get; set; }
    //     public int amount_tax { get; set; }
    // }
}

// {
//   "id": "evt_1T9m4r8iqvn4Z7bkjQaX8VZ8",
//   "object": "event",
//   "api_version": "2025-09-30.clover",
//   "created": 1773232889,
//   "data": {
//     "object": {
//       "id": "cs_test_a1HYntSn5hEeE7rLxp8dU1I4nWIg0kT21S3D34EAOSDPwDMPf3CKtSjkiT",
//       "object": "checkout.session",
//       "adaptive_pricing": {
//         "enabled": true
//       },
//       "after_expiration": null,
//       "allow_promotion_codes": null,
//       "amount_subtotal": 12000,
//       "amount_total": 12000,
//       "automatic_tax": {
//         "enabled": false,
//         "liability": null,
//         "provider": null,
//         "status": null
//       },
//       "billing_address_collection": null,
//       "branding_settings": {
//         "background_color": "#ffffff",
//         "border_style": "rounded",
//         "button_color": "#0074d4",
//         "display_name": "",
//         "font_family": "default",
//         "icon": null,
//         "logo": null
//       },
//       "cancel_url": "http://localhost:3000/payment/cancel",
//       "client_reference_id": null,
//       "client_secret": null,
//       "collected_information": null,
//       "consent": null,
//       "consent_collection": null,
//       "created": 1773232869,
//       "currency": "thb",
//       "currency_conversion": null,
//       "custom_fields": [],
//       "custom_text": {
//         "after_submit": null,
//         "shipping_address": null,
//         "submit": null,
//         "terms_of_service_acceptance": null
//       },
//       "customer": null,
//       "customer_account": null,
//       "customer_creation": "if_required",
//       "customer_details": {
//         "address": {
//           "city": null,
//           "country": null,
//           "line1": null,
//           "line2": null,
//           "postal_code": null,
//           "state": null
//         },
//         "business_name": null,
//         "email": "tuatang@gmail.com",
//         "individual_name": null,
//         "name": null,
//         "phone": null,
//         "tax_exempt": "none",
//         "tax_ids": []
//       },
//       "customer_email": null,
//       "discounts": [],
//       "expires_at": 1773319269,
//       "integration_identifier": null,
//       "invoice": null,
//       "invoice_creation": {
//         "enabled": false,
//         "invoice_data": {
//           "account_tax_ids": null,
//           "custom_fields": null,
//           "description": null,
//           "footer": null,
//           "issuer": null,
//           "metadata": {},
//           "rendering_options": null
//         }
//       },
//       "livemode": false,
//       "locale": null,
//       "metadata": {
//         "bill_id": "xxx"
//       },
//       "mode": "payment",
//       "origin_context": null,
//       "payment_intent": "pi_3T9m4h8iqvn4Z7bk0dqlrwaU",
//       "payment_link": null,
//       "payment_method_collection": "if_required",
//       "payment_method_configuration_details": null,
//       "payment_method_options": {},
//       "payment_method_types": [
//         "promptpay"
//       ],
//       "payment_status": "paid",
//       "permissions": null,
//       "phone_number_collection": {
//         "enabled": false
//       },
//       "recovered_from": null,
//       "saved_payment_method_options": null,
//       "setup_intent": null,
//       "shipping_address_collection": null,
//       "shipping_cost": null,
//       "shipping_options": [],
//       "status": "complete",
//       "submit_type": null,
//       "subscription": null,
//       "success_url": "http://localhost:3000/payment/success?session_id={CHECKOUT_SESSION_ID}&payment_method=promptpay",
//       "total_details": {
//         "amount_discount": 0,
//         "amount_shipping": 0,
//         "amount_tax": 0
//       },
//       "ui_mode": "hosted",
//       "url": null,
//       "wallet_options": null
//     }
//   },
//   "livemode": false,
//   "pending_webhooks": 1,
//   "request": {
//     "id": null,
//     "idempotency_key": null
//   },
//   "type": "checkout.session.completed"
// }