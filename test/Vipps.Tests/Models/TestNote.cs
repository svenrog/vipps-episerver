using EPiServer.Commerce.Order;
using System;

namespace Vipps.Test.Models
{
    public class TestNote : IOrderNote
    {
        public int? OrderNoteId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Type { get; set; }
        public int? LineItemId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime Created { get; set; }
    }
}
