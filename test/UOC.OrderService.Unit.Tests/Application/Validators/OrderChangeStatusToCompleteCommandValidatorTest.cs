using UOC.OrderService.Application.Messages.Commands;
using UOC.OrderService.Application.Validators.Commands;

namespace UOC.OrderService.Unit.Testes.Application
{
    public class OrderChangeStatusToCompleteCommandValidatorTest
    {
        private readonly OrderChangeStatusToCompleteCommandValidator validator;

        public OrderChangeStatusToCompleteCommandValidatorTest()
        {
            this.validator = new OrderChangeStatusToCompleteCommandValidator();
        }

        [Trait("Category", "Unit")]
        [Trait("Layer", "Application")]
        [Theory]
        [MemberData(nameof(ValidateCases))]
        public void OrderChangeStatusToCompleteCommandValidator_ValidateIsCalled_ValidationResultIsTheExpectedOne(
            Guid orderId,
            Guid correlationId,
            bool expectedResult)
        {
            // Arrange
            var command = new OrderChangeStatusToCompleteCommand()
            {
                OrderId = orderId,
                CorrelationId = correlationId
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
                new object[] { Guid.Empty, Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), false },
                new object[] { Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Empty, false },
                new object[] { Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), Guid.Parse("ef7259e9-95d2-4353-8d89-c48abfa723c2"), true },
            };
        }
    }
}