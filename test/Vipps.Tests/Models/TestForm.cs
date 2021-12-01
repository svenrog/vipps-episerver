using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using Mediachase.Commerce.Orders;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Vipps.Test.Models
{
    public class TestForm : IOrderForm
    {
        private readonly IOrderGroup _parent;

        public TestForm(IOrderGroup parent, ICollection<IPayment> payments = null, ICollection<IShipment> shipments = null)
        {
            _parent = parent;

            Payments = payments ?? new List<IPayment>();
            Shipments = shipments ?? new List<IShipment>();
            Promotions = new List<PromotionInformation>();
            CouponCodes = new List<string>();
            Properties = new Hashtable();
        }

        public int OrderFormId => _parent.OrderLink.OrderGroupId;

        public decimal AuthorizedPaymentTotal { get => GetAuthorizedTotal(); set => throw new NotSupportedException(); }
        public decimal CapturedPaymentTotal { get => GetCapturedTotal(); set => throw new NotSupportedException(); }
        public decimal HandlingTotal { get => 0; set => throw new NotSupportedException(); }
        public string Name { get => TestConstants.TestOrderFormName; set => throw new NotSupportedException(); }

        public ICollection<IShipment> Shipments { get; }

        public IList<PromotionInformation> Promotions { get; }

        public ICollection<string> CouponCodes { get; }

        public ICollection<IPayment> Payments { get; }

        public bool PricesIncludeTax => _parent.PricesIncludeTax;

        public IOrderGroup ParentOrderGroup => _parent;

        public Hashtable Properties { get; }

        private decimal GetAuthorizedTotal()
        {
            var result = 0m;

            foreach (var payment in Payments)
            {
                if (payment.TransactionType != nameof(TransactionType.Authorization))
                    continue;

                if (payment.Status != nameof(PaymentStatus.Processed))
                    continue;

                result += payment.Amount;
            }

            return result;
        }

        private decimal GetCapturedTotal()
        {
            var result = 0m;

            foreach (var payment in Payments)
            {
                if (payment.TransactionType != nameof(TransactionType.Capture) && 
                    payment.TransactionType != nameof(TransactionType.CaptureOnly))
                    continue;

                if (payment.Status != nameof(PaymentStatus.Processed))
                    continue;

                result += payment.Amount;
            }

            return result;
        }
    }
}
