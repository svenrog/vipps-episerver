﻿using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using Vipps.Models.Partials;
using Vipps.Models.RequestModels;

namespace Vipps.Helpers
{
    public static class AddressHelper
    {
        public static IOrderAddress ShippingRequestToOrderAddress(ShippingRequest shippingRequest, ICart cart, IOrderGroupFactory orderGroupFactory = null)
        {
            if (orderGroupFactory == null)
                orderGroupFactory = ServiceLocator.Current.GetInstance<IOrderGroupFactory>();

            var addressId = $"{shippingRequest.AddressLine1}{shippingRequest.AddressLine2}{shippingRequest.City}";
            var orderAddress = cart.CreateOrderAddress(orderGroupFactory, addressId);

            orderAddress.City = shippingRequest.City;
            orderAddress.CountryCode = "NOR";
            orderAddress.CountryName = shippingRequest.Country;
            orderAddress.Line1 = shippingRequest.AddressLine1;
            orderAddress.Line2 = shippingRequest.AddressLine2;
            orderAddress.PostalCode = shippingRequest.PostalCode;

            return orderAddress;
        }

        public static IOrderAddress UserDetailsAndShippingDetailsToOrderAddress(UserDetails userDetails, ShippingDetails shippingDetails, ICart cart, IOrderGroupFactory orderGroupFactory = null)
        {
            if (orderGroupFactory == null)
                orderGroupFactory = ServiceLocator.Current.GetInstance<IOrderGroupFactory>();

            var addressId = $"{shippingDetails.Address.AddressLine1}{shippingDetails.Address.AddressLine2}{shippingDetails.Address.City}";
            var orderAddress = cart.CreateOrderAddress(orderGroupFactory, addressId);

            orderAddress.City = shippingDetails.Address.City;
            orderAddress.CountryCode = "NOR";
            orderAddress.CountryName = shippingDetails.Address.Country;
            orderAddress.Line1 = shippingDetails.Address.AddressLine1;
            orderAddress.Line2 = shippingDetails.Address.AddressLine2;
            orderAddress.PostalCode = shippingDetails.Address.ZipCode;
            orderAddress.FirstName = userDetails.FirstName;
            orderAddress.LastName = userDetails.LastName;
            orderAddress.DaytimePhoneNumber = userDetails.MobileNumber;
            orderAddress.Email = userDetails.Email;

            return orderAddress;
        }
    }
}
