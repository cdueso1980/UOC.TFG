using UOC.OrderService.Domain;

namespace UOC.OrderService.Unit.Testes.Domain
{
    public class OrderTest
    {
        private readonly Order order;

        public OrderTest()
        {
            order = new Order(Guid.NewGuid(), new System.Collections.Generic.List<(Guid ProductId, decimal Amount)> ()
            {
                (Guid.NewGuid(), 15.66m),
                (Guid.NewGuid(), 1000.22m),
            });
        }

        [Fact]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
		public void Order_ChangeStatusToPayment_OK() 
        {
            // Act
            order.ChangeStatusToPayment();

            // Assert 
            Assert.Equal(OrderStatus.Payment, order.Status);
        }

        [Theory]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
        [MemberData(nameof(GetAllStatusDataForCreatedStateInitial))]
		public void Order_ChangeStatusToPayment_Throw_Exception_On_Bad_Status(OrderStatus currentStatus) 
        {
            // Arrange
            order.SetPropertyValue(r => r.Status, currentStatus);

             // Act
            Assert.Throws<DomainException>(() => order.ChangeStatusToPayment());

            // Assert 
            Assert.Equal(currentStatus, order.Status);
        }

        [Fact]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
		public void Order_ChangeStatusToCompleted_OK() 
        {
            // Arrange
            order.SetPropertyValue(r => r.Status, OrderStatus.Payment);

            // Act
            order.ChangeStatusToCompleted();

            // Assert 
            Assert.Equal(OrderStatus.Completed, order.Status);
        }

        [Theory]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
        [MemberData(nameof(GetAllStatusDataForPaymentStateInitial))]
		public void Order_ChangeStatusToCompleted_Throw_Exception_On_Bad_Status(OrderStatus currentStatus) 
        {
            // Arrange
            order.SetPropertyValue(r => r.Status, currentStatus);

             // Act
            Assert.Throws<DomainException>(() => order.ChangeStatusToCompleted());

            // Assert 
            Assert.Equal(currentStatus, order.Status);
        }

        [Fact]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
		public void Order_ChangeStatusToPaymentRejected_OK() 
        {
            // Arrange
            order.SetPropertyValue(r => r.Status, OrderStatus.Payment);

            // Act
            order.ChangeStatusToPaymentRejected();

            // Assert 
            Assert.Equal(OrderStatus.PaymentRejected, order.Status);
        }

        [Theory]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
        [MemberData(nameof(GetAllStatusDataForPaymentStateInitial))]
		public void Order_ChangeStatusToPaymentRejected_Throw_Exception_On_Bad_Status(OrderStatus currentStatus) 
        {
            // Arrange
            order.SetPropertyValue(r => r.Status, currentStatus);

             // Act
            Assert.Throws<DomainException>(() => order.ChangeStatusToPaymentRejected());

            // Assert 
            Assert.Equal(currentStatus, order.Status);
        }


        public static IEnumerable<object[]> GetAllStatusDataForCreatedStateInitial() 
        {
			return new List<object[]>
			{
				new object[] { OrderStatus.Completed },
				new object[] { OrderStatus.Payment },
				new object[] { OrderStatus.PaymentRejected }
			};
		}

        public static IEnumerable<object[]> GetAllStatusDataForPaymentStateInitial() 
        {
			return new List<object[]>
			{
				new object[] { OrderStatus.Completed },
				new object[] { OrderStatus.Created },
				new object[] { OrderStatus.PaymentRejected }
			};
		}
    }
}