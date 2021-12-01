using System.Linq;
using System;
using Vipps.Services;
using Xunit;
using System.Threading.Tasks;
using Moq;
using EPiServer.Commerce.Order;
using Vipps.Test.Models;
using Vipps.Models.ResponseModels;
using Vipps.Models.Partials;
using System.Collections.Generic;
using Vipps.Models;
using Vipps.Test.Services;
using Vipps.Extensions;
using Vipps.Helpers;
using Mediachase.Commerce.Orders;

namespace Vipps.Test.Tests
{
    public class OrderProcessorTests
    {
        [Fact]
        public void FetchAndProcessOrderDetails_ShouldExitWhenFindingPurchaseOrder()
        {
            var orderId = "PO1";
            var contactId = Guid.NewGuid();
            var vippsService = new Mock<IVippsService>();

            vippsService.Setup(x => x.GetPurchaseOrderByOrderId(orderId))
                        .Returns(() => new TestOrder(1, contactId));

            var orderRepository = new Mock<IOrderRepository>();
            var orderGroupFactory = new Mock<IOrderGroupFactory>();
            var synchronizer = new DefaultVippsOrderSynchronizer();
            var logger = new NullLogger();

            var processor = new DefaultVippsOrderProcessor(orderRepository.Object, orderGroupFactory.Object, vippsService.Object, synchronizer, logger);

            var response = processor.FetchAndProcessOrderDetails(orderId, contactId, TestConstants.TestMarketName, TestConstants.TestCartName);

            Assert.NotNull(response);
            Assert.NotNull(response.PurchaseOrder);
            Assert.Equal(ProcessResponseErrorType.NONE, response.ProcessResponseErrorType);
            Assert.Null(response.ErrorMessage);
        }

        [Fact]
        public async Task FetchAndProcessOrderDetailsAsync_ShouldExitWhenFindingPurchaseOrder()
        {
            var orderId = "PO1";
            var contactId = Guid.NewGuid();
            var vippsService = new Mock<IVippsService>();

            vippsService.Setup(x => x.GetPurchaseOrderByOrderId(orderId))
                        .Returns(() => new TestOrder(1, contactId));

            var orderRepository = new Mock<IOrderRepository>();
            var orderGroupFactory = new Mock<IOrderGroupFactory>();
            var synchronizer = new DefaultVippsOrderSynchronizer();
            var logger = new NullLogger();

            var processor = new DefaultVippsOrderProcessor(orderRepository.Object, orderGroupFactory.Object, vippsService.Object, synchronizer, logger);

            var response = await processor.FetchAndProcessOrderDetailsAsync(orderId, contactId, TestConstants.TestMarketName, TestConstants.TestCartName);

            Assert.NotNull(response);
            Assert.NotNull(response.PurchaseOrder);
            Assert.Equal(ProcessResponseErrorType.NONE, response.ProcessResponseErrorType);
            Assert.Null(response.ErrorMessage);
        }

        [Fact]
        public void FetchAndProcessOrderDetails_ShouldExitWhenFindingLockedCart()
        {
            var orderId = "PO1";
            var contactId = Guid.NewGuid();
            var lockedCart = new TestOrder(1, contactId);

            lockedCart.SetOrderProcessing(true);

            var vippsService = new Mock<IVippsService>();

            vippsService.Setup(x => x.GetCartByContactId(contactId, TestConstants.TestMarketName, TestConstants.TestCartName))
                        .Returns(() => lockedCart);

            var orderRepository = new Mock<IOrderRepository>();
            var orderGroupFactory = new Mock<IOrderGroupFactory>();
            var synchronizer = new DefaultVippsOrderSynchronizer();
            var logger = new NullLogger();

            var processor = new DefaultVippsOrderProcessor(orderRepository.Object, orderGroupFactory.Object, vippsService.Object, synchronizer, logger);

            var response = processor.FetchAndProcessOrderDetails(orderId, contactId, TestConstants.TestMarketName, TestConstants.TestCartName);

            Assert.NotNull(response);
            Assert.Null(response.PurchaseOrder);
            Assert.Equal(ProcessResponseErrorType.OTHER, response.ProcessResponseErrorType);
            Assert.NotNull(response.ErrorMessage);
        }

        [Fact]
        public async Task FetchAndProcessOrderDetailsAsync_ShouldExitWhenFindingLockedCart()
        {
            var orderId = "PO1";
            var contactId = Guid.NewGuid();
            var lockedCart = new TestOrder(1, contactId);

            lockedCart.SetOrderProcessing(true);

            var vippsService = new Mock<IVippsService>();

            vippsService.Setup(x => x.GetCartByContactId(contactId, TestConstants.TestMarketName, TestConstants.TestCartName))
                        .Returns(() => lockedCart);

            var orderRepository = new Mock<IOrderRepository>();
            var orderGroupFactory = new Mock<IOrderGroupFactory>();
            var synchronizer = new DefaultVippsOrderSynchronizer();
            var logger = new NullLogger();

            var processor = new DefaultVippsOrderProcessor(orderRepository.Object, orderGroupFactory.Object, vippsService.Object, synchronizer, logger);

            var response = await processor.FetchAndProcessOrderDetailsAsync(orderId, contactId, TestConstants.TestMarketName, TestConstants.TestCartName);

            Assert.NotNull(response);
            Assert.Null(response.PurchaseOrder);
            Assert.Equal(ProcessResponseErrorType.OTHER, response.ProcessResponseErrorType);
            Assert.NotNull(response.ErrorMessage);
        }

        [Fact]
        public void FetchAndProcessOrderDetails_CanProcess()
        {
            var orderId = "PO1";
            var orderGroupId = 1;
            var contactId = Guid.NewGuid();
            var paymentAmount = 100m;

            var testPayment = new TestPayment(orderId, paymentAmount);
            var testOrder = new TestOrder(orderGroupId, contactId, new [] { testPayment });
            var testShipment = testOrder.GetFirstShipment();

            var vippsService = new Mock<IVippsService>();

            vippsService.Setup(x => x.GetCartByContactId(contactId, TestConstants.TestMarketName, TestConstants.TestCartName))
                        .Returns(() => testOrder);

            vippsService.Setup(x => x.GetOrderDetailsAsync(It.IsAny<string>(), TestConstants.TestMarketName))
                        .Returns((string x, string y) => Task.FromResult(new DetailsResponse
                        {
                            OrderId = x,
                            UserDetails = new UserDetails
                            {
                                FirstName = "Grodan",
                                LastName = "Boll",
                                DateOfBirth = "19010101",
                                Email = "tester@getadigital.com",
                                MobileNumber = "+4670123456",
                                Ssn = "1234"
                            },
                            ShippingDetails = new ShippingDetails
                            {
                                ShippingMethod = testShipment.ShippingMethodName,
                                Address = new Address
                                {
                                    AddressLine1 = "Gatan 1",
                                    City = "Gävle",
                                    PostCode = "80268",
                                    Country = "SE",
                                }
                            },
                            TransactionLogHistory = new List<TransactionLogHistory>
                            {
                                new TransactionLogHistory
                                {
                                    Amount = AmountHelper.FormatAmountToVipps(testPayment.Amount),
                                    Operation = $"{VippsDetailsResponseOperation.RESERVE}",
                                    OperationSuccess = true,
                                    TimeStamp = DateTime.UtcNow,
                                    RequestId = x,
                                    TransactionId = x,
                                    TransactionText = string.Empty
                                }
                            }
                        }));

            var orderGroupFactory = new Mock<IOrderGroupFactory>();

            orderGroupFactory.Setup(x => x.CreateOrderAddress(It.IsAny<IOrderGroup>()))
                             .Returns(new TestAdress());

            orderGroupFactory.Setup(x => x.CreateOrderNote(It.IsAny<IOrderGroup>()))
                             .Returns(new TestNote());

            var orderRepository = new Mock<IOrderRepository>();

            orderRepository.Setup(x => x.SaveAsPurchaseOrder(It.IsAny<ICart>()))
                           .Returns((ICart x) => x.OrderLink);

            orderRepository.Setup(x => x.Load<IPurchaseOrder>(orderGroupId))
                           .Returns(testOrder);

            var synchronizer = new DefaultVippsOrderSynchronizer();
            var logger = new NullLogger();

            var processor = new DefaultVippsOrderProcessor(orderRepository.Object, orderGroupFactory.Object, vippsService.Object, synchronizer, logger);

            var response = processor.FetchAndProcessOrderDetails(orderId, contactId, TestConstants.TestMarketName, TestConstants.TestCartName);

            Assert.NotNull(response);
            Assert.NotNull(response.PurchaseOrder);
            Assert.Equal(ProcessResponseErrorType.NONE, response.ProcessResponseErrorType);
            
            var firstForm = response.PurchaseOrder.GetFirstForm();

            Assert.Equal(paymentAmount, firstForm.AuthorizedPaymentTotal);

            var firstPayment = response.PurchaseOrder.GetFirstPayment();

            Assert.Equal(nameof(PaymentStatus.Processed), firstPayment.Status);
        }
    }
}
