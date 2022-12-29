using UOC.PaymentService.Application.Messages.Commands;
using UOC.PaymentService.Application.Validators.Commands;

namespace UOC.PaymentService.Unit.Testes.Application
{
    public class PayOrderCommandValidatorTest
    {
        private readonly PayOrderCommandValidator validator;

        public PayOrderCommandValidatorTest()
        {
            this.validator = new PayOrderCommandValidator();
        }

        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        [Theory]
        [MemberData(nameof(ValidateCases))]
        public void PayOrderCommandValidatorTest_ValidateIsCalled_ValidationResultIsTheExpectedOne(
            Guid orderId,
            Guid correlationId,
            Guid customerId,
            decimal totalAmount,
            bool expectedResult)
        {
            // Arrange
            var command = new PayOrderCommand()
            {
                OrderId = orderId,
                CorrelationId = correlationId,
                CustomerId = customerId,
                TotalAmount = totalAmount
            };  

            // Act
            var validationResult = this.validator.Validate(command);

            // Assert
            Assert.Equal(expectedResult, validationResult.IsValid);
        }

        public static IEnumerable<object[]> ValidateCases()
        {
            return new List<object[]>
            {
                new object[] { Guid.Empty, Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), 1500m, false },
                new object[] { Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Empty, Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), 1500m, false },
                new object[] { Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Empty, 1500m, false },
                new object[] { Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), 0m, false },
                new object[] { Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), 1500m, true },
            };
        }
    }
}