using AuthorizeNet;
using EPiServer.Commerce.Order;
using Mediachase.Commerce.Orders;
using System;
using System.Collections;
using PaymentTransactionType = Mediachase.Commerce.Orders.TransactionType;

namespace Vipps.Test.Models
{
    public class TestPayment : IPayment
    {
        private readonly string _orderId;

        public TestPayment(string orderId, decimal amount, string status = null)
        {
            _orderId = orderId;
            
            Amount = amount;
            Status = status ?? nameof(PaymentStatus.Pending);
            Properties = new Hashtable();
        }

        public decimal Amount { get; set; }
        public string AuthorizationCode { get; set; }
        public IOrderAddress BillingAddress { get; set; }
        public string CustomerName { get; set; }

        public string ImplementationClass => nameof(TestPayment);

        public int PaymentId => -1;

        public Guid PaymentMethodId { get => Guid.Empty; set => throw new NotSupportedException(); }
        public string PaymentMethodName { get => VippsConstants.VippsSystemKeyword; set => throw new NotSupportedException(); }
        public PaymentType PaymentType { get => PaymentType.Other; set => throw new NotSupportedException(); }
        public string ProviderTransactionID { get; set; }
        public string Status { get; set; }
        public string TransactionID { get => _orderId; set => throw new NotSupportedException(); }
        public string TransactionType { get => nameof(PaymentTransactionType.Authorization); set => throw new NotSupportedException(); }
        public string ValidationCode { get; set; }

        public Hashtable Properties { get; }
    }
}
