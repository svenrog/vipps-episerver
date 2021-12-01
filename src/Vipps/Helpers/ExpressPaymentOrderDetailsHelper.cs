using System;
using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using Vipps.Models.ResponseModels;

namespace Vipps.Helpers
{
    public static class ExpressPaymentOrderDetailsHelper
    {
        public static void EnsureExpressPaymentAndShipping(ICart cart, IPayment payment, DetailsResponse orderDetails, IOrderRepository orderRepository = null, IOrderGroupFactory orderGroupFactory = null)
        {
            if (orderRepository == null)
                orderRepository = ServiceLocator.Current.GetInstance<IOrderRepository>();

            if (orderGroupFactory == null)
                orderGroupFactory = ServiceLocator.Current.GetInstance<IOrderGroupFactory>();

            if (cart.GetFirstShipment().ShippingMethodId == default(Guid) ||
                cart.GetFirstShipment().ShippingAddress == null ||
                payment?.BillingAddress == null)
            {
                EnsureShipping(cart, orderDetails, orderGroupFactory);
                EnsureBillingAddress(payment, cart, orderDetails, orderGroupFactory);

                orderRepository.Save(cart);
            }
        }

        private static void EnsureBillingAddress(IPayment payment, ICart cart, DetailsResponse details, IOrderGroupFactory orderGroupFactory)
        {
            if (payment.BillingAddress == null)
            {
                payment.BillingAddress =
                    AddressHelper.UserDetailsAndShippingDetailsToOrderAddress(details.UserDetails,
                        details.ShippingDetails, cart, orderGroupFactory);
            }
        }

        private static void EnsureShipping(ICart cart, DetailsResponse details, IOrderGroupFactory orderGroupFactory)
        {
            

            var shipment = cart.GetFirstShipment();
            if (shipment.ShippingMethodId == default(Guid))
            {
                if (details?.ShippingDetails?.ShippingMethodId != null)
                {
                    shipment.ShippingMethodId = new Guid(details.ShippingDetails.ShippingMethodId);
                }
            }

            if (shipment.ShippingAddress == null)
            {
                shipment.ShippingAddress =
                    AddressHelper.UserDetailsAndShippingDetailsToOrderAddress(details?.UserDetails,
                        details?.ShippingDetails, cart, orderGroupFactory);
            }
        }
    }
}
