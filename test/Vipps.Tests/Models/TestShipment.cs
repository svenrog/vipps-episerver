using EPiServer.Commerce.Order;
using Mediachase.Commerce.Orders;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Vipps.Test.Models
{
    public class TestShipment : IShipment
    {
        private readonly IOrderGroup _parent;

        public TestShipment(IOrderGroup parent)
        {
            _parent = parent;

            LineItems = new List<ILineItem>();
            Properties = new Hashtable();
        }

        public int ShipmentId => -1;
        public Guid ShippingMethodId { get => Guid.Empty; set => throw new NotSupportedException(); }

        public string ShippingMethodName => TestConstants.TestShipmentMethodName;

        public IOrderAddress ShippingAddress { get; set; }
        public string ShipmentTrackingNumber { get; set; }
        public OrderShipmentStatus OrderShipmentStatus { get; set; }
        public int? PickListId { get; set; }
        public string WarehouseCode { get; set; }

        public ICollection<ILineItem> LineItems { get; }

        public IOrderGroup ParentOrderGroup => _parent;

        public Hashtable Properties { get; }
    }
}
