using UOC.PaymentService.Domain;

namespace UOC.PaymentService.Unit.Testes.Domain
{
    public class OrderTest
    {
        private readonly Payment payment;

        public OrderTest()
        {
            payment = new Payment(Guid.NewGuid(), Guid.NewGuid(), 1500m);
        }

        [Fact]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
		public void Payment_ChangeStatusToCompleted_OK() 
        {
            // Act
            payment.ChangeStatusToCompleted();

            // Assert 
            Assert.Equal(PaymentStatus.Completed, payment.Status);
        }

        [Theory]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
        [MemberData(nameof(GetAllStatusDataForCreatedStateInitial))]
		public void Payment_ChangeStatusToCompleted_Throw_Exception_On_Bad_Status(PaymentStatus currentStatus) 
        {
            // Arrange
            payment.SetPropertyValue(r => r.Status, currentStatus);

             // Act
            Assert.Throws<DomainException>(() => payment.ChangeStatusToCompleted());

            // Assert 
            Assert.Equal(currentStatus, payment.Status);
        }

        [Fact]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
		public void Payment_ChangeStatusToRejected_OK() 
        {
            // Act
            payment.ChangeStatusToRejected();

            // Assert 
            Assert.Equal(PaymentStatus.Rejected, payment.Status);
        }

        [Theory]
        [Trait("Category", "Unit")]
		[Trait("Layer", "Domain")]
        [MemberData(nameof(GetAllStatusDataForCreatedStateInitial))]
		public void Order_ChangeStatusToRejected_Throw_Exception_On_Bad_Status(PaymentStatus currentStatus) 
        {
            // Arrange
            payment.SetPropertyValue(r => r.Status, currentStatus);

             // Act
            Assert.Throws<DomainException>(() => payment.ChangeStatusToRejected());

            // Assert 
            Assert.Equal(currentStatus, payment.Status);
        }

        public static IEnumerable<object[]> GetAllStatusDataForCreatedStateInitial() 
        {
			return new List<object[]>
			{
				new object[] { PaymentStatus.Completed },
				new object[] { PaymentStatus.Rejected },
			};
		}
    }
}