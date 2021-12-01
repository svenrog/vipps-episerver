using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using Vipps.Extensions;

namespace Vipps.Helpers
{
    public static class OrderNoteHelper
    {
        public static void AddNoteAndSaveChanges(IOrderGroup orderGroup, IPayment payment, string transactionType, string noteMessage, IOrderRepository orderRepository = null, IOrderGroupFactory orderGroupFactory = null)
        {
            if (orderRepository == null)
                orderRepository = ServiceLocator.Current.GetInstance<IOrderRepository>();

            if (orderGroupFactory == null)
                orderGroupFactory = ServiceLocator.Current.GetInstance<IOrderGroupFactory>();

            var noteTitle = $"{payment.PaymentMethodName} {transactionType.ToLower()}";

            orderGroup.AddNote(noteTitle, $"Payment {transactionType.ToLower()}: {noteMessage}", orderRepository, orderGroupFactory);
        }

        public static void AddNoteAndSaveChanges(IOrderGroup orderGroup, string noteTitle,
            string noteMessage, IOrderRepository orderRepository = null, IOrderGroupFactory orderGroupFactory = null)
        {
            if (orderRepository == null)
                orderRepository = ServiceLocator.Current.GetInstance<IOrderRepository>();

            if (orderGroupFactory == null)
                orderGroupFactory = ServiceLocator.Current.GetInstance<IOrderGroupFactory>();

            orderGroup.AddNote(noteTitle, noteMessage, orderRepository, orderGroupFactory);
        }
    }
}