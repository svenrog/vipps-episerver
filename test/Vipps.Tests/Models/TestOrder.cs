using EPiServer.Commerce.Order;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Vipps.Test.Models
{
    public class TestOrder : IPurchaseOrder, ICart
    {
        private const string _orderTypeName = "test";
        
        private readonly int _orderGroupId;
        private readonly Guid _customerId;
        private readonly DateTime _created;
        private readonly Hashtable _properties;
        
        public TestOrder(int orderGroupId, Guid? customerId = null, ICollection<IPayment> payments = null)
        {
            _orderGroupId = orderGroupId;
            _customerId = customerId ?? Guid.NewGuid();
            _created = DateTime.UtcNow;
            _properties = new Hashtable();

            var shipments = new List<IShipment>
            {
                new TestShipment(this)
            };                

            Market = new TestMarket(TestConstants.TestMarketName);

            Notes = new List<IOrderNote>();
            Forms = new List<IOrderForm>
            {
                new TestForm(this, payments, shipments)
            };
        }

        public string OrderNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public ICollection<IReturnOrderForm> ReturnForms => null;
        public OrderReference OrderLink => new OrderReference(_orderGroupId, _orderTypeName, _customerId, typeof(TestOrder));
        public ICollection<IOrderForm> Forms { get; }
        public ICollection<IOrderNote> Notes { get; }
        public IMarket Market { get; set; }
        public string Name { get => _orderTypeName; set => throw new NotSupportedException(); }
        public Guid? Organization { get => null; set => throw new NotSupportedException(); }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.InProgress;
        public Currency Currency { get => Market.DefaultCurrency; set => throw new NotSupportedException(); }
        public Guid CustomerId { get => _customerId; set => throw new NotSupportedException(); }

        public DateTime Created => _created;

        public DateTime? Modified => null;

        public MarketId MarketId { get => Market.MarketId; set => throw new NotSupportedException(); }
        public string MarketName { get => Market.MarketName; set => throw new NotSupportedException(); }
        public bool PricesIncludeTax { get => Market.PricesIncludeTax; set => throw new NotSupportedException(); }
        public Hashtable Properties => _properties;
    }
}
