﻿using AnglingClubWebServices.Interfaces;
using AnglingClubWebServices.Models;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Linq;

namespace AnglingClubWebServices.Services
{
    public class PaymentService : IPaymentsService
    {
        public PaymentService(IOptions<StripeOptions> opts)
        {
            StripeConfiguration.ApiKey = opts.Value.StripeApiKey;
        }

        public List<Payment> GetPayments()
        {
            var payments = new List<Payment>();

            var sessionService = new SessionService();
            StripeList<Session> sessions = sessionService.List();

            var completeSessions = sessions
                                    .Where(x => x.PaymentStatus == "paid" && x.CustomFields.Any())
                                    .Select(x => new { x.Id, x.PaymentIntentId, Name = x.CustomFields.First().Text.Value, x.CustomerDetails, x.AmountTotal, x.Created, x.Status, x.PaymentStatus });

            var service = new ChargeService();
            StripeList<Charge> charges = service.List();

            var refundedCharges = charges.Where(x => x.Refunded == true).Select(x => new { x.PaymentIntentId });

            foreach (var session in completeSessions)
            {
                StripeList<LineItem> lineItems = sessionService.ListLineItems(session.Id);

                var purchase = lineItems.First().Description;

                var productService = new ProductService();
                var product = productService.Get(lineItems.First().Price.ProductId);
                var category = product.Metadata.Where(m => m.Key == "Category").First().Value;
                var charge = charges.FirstOrDefault(c => c.PaymentIntentId == session.PaymentIntentId);
                var address = charge.Shipping != null ? charge.Shipping.Address : charge.BillingDetails.Address;


                payments.Add(new Payment
                {
                    SessionId = session.Id,
                    MembersName = session.Name,
                    Email = session.CustomerDetails.Email,
                    Category = category.GetValueFromDescription<PaymentType>(),
                    Purchase = purchase,
                    Amount = session.AmountTotal.Value / 100,
                    PaidOn = session.Created.ToLocalTime(),
                    Status = refundedCharges.Any(x => x.PaymentIntentId == session.PaymentIntentId) ? "refunded" : $"{session.Status} ({session.PaymentStatus})",
                    CardHoldersName = charge.BillingDetails.Name,
                    ShippingAddress = $"{address.Line1}, {(address.Line2 != null ? address.Line2 + ", " : "")}{address.City}, {address.PostalCode}"
                });
            }

            return payments;
        }
    }
}