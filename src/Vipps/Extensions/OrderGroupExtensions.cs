using System;
using System.Linq;
using EPiServer.Commerce.Order;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Vipps.Helpers;
using Vipps.Models;

namespace Vipps.Extensions
{
    public static class OrderGroupExtensions
    {
        public static VippsPaymentType GetVippsPaymentType(this IOrderGroup orderGroup)
        {
            return PaymentTypeHelper.GetVippsPaymentType(orderGroup);
        }

        public static void SetOrderProcessing(this IOrderGroup orderGroup, bool value)
        {
            if (orderGroup == null) return;
            orderGroup.Properties[VippsConstants.VippsIsProcessingOrderField] = value;
        }

        public static bool IsProcessingOrder(this IOrderGroup orderGroup)
        {
            return orderGroup?.Properties[VippsConstants.VippsIsProcessingOrderField] as bool? ?? false;
        }

        public static IPayment GetFirstPayment(this IOrderGroup orderGroup)
        {
            return orderGroup.Forms.SelectMany(x => x.Payments).FirstOrDefault();
        }

        public static IPayment GetFirstPayment(this IOrderGroup orderGroup, Func<IPayment, bool> predicate)
        {
            return orderGroup.Forms.SelectMany(x => x.Payments).FirstOrDefault(predicate);
        }

        public static void AddNote(this IOrderGroup orderGroup, string noteTitle, string noteMessage, IOrderRepository orderRepository, IOrderGroupFactory orderGroupFactory)
        {
            var note = orderGroupFactory.CreateOrderNote(orderGroup);

            note.CustomerId = CustomerContext.Current.CurrentContactId;
            note.Type = OrderNoteTypes.Custom.ToString();
            note.Title = noteTitle;
            note.Detail = noteMessage;
            note.Created = DateTime.UtcNow;

            orderGroup.Notes.Add(note);

            orderRepository.Save(orderGroup);
        }
    }
}
