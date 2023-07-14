using MediatR;
using PaymentGateway.Domain.Clients;
using PaymentGateway.Domain.Concrete;
using PaymentGateway.Domain.Repository;

namespace Domain.Tests
{
    public class ProcessPaymentRequestHandlerTests
    {
        [Test]
        public async Task HandleShouldReturnProcessedPaymentIfCkoExecutionIsSuccessfull()
        {
            // Arrange
            var mocker = new AutoMocker();
            var request = Helpers.BuildRequest();

            mocker.Use<IRepository<Payment>>(new InmemoryRepository<Payment>());
            mocker.GetMock<ICkoClient>()
                  .Setup(x => x.ExecuteTransactionAsync(request.Amount, request.Card))
                  .ReturnsAsync(new Ok<Unit>(Unit.Value));

            var handler = mocker.CreateInstance<ProcessPaymentRequestHandler>();

            // Act
            var result = await handler.Handle(request, new CancellationToken());

            // Assert
            result.Should().BeOfType<Ok<Payment>>();
            var payment = result.Unwrap();
            payment.Amount.Should().BeEquivalentTo(request.Amount);
            payment.Card.Should().BeEquivalentTo(request.Card);
            payment.MerchantId.Should().Be(request.MerchantId);
            payment.Status.Should().Be(PaymentStatus.Processed);
        }

        [Test]
        public async Task HandleShouldReturnFailedPaymentIfCkoExecutionFails()
        {
            // Arrange
            var mocker = new AutoMocker();
            var request = Helpers.BuildRequest();

            var repo = new InmemoryRepository<Payment>();
            mocker.Use<IRepository<Payment>>(repo);

            mocker.GetMock<ICkoClient>()
                  .Setup(x => x.ExecuteTransactionAsync(request.Amount, request.Card))
                  .ReturnsAsync(new Error<Unit>(new Exception()));

            var handler = mocker.CreateInstance<ProcessPaymentRequestHandler>();

            // Act
            var result = await handler.Handle(request, new CancellationToken());

            // Assert
            result.Should().BeOfType<Error<Payment>>();
            repo.GetAll().Single().Status.Should().Be(PaymentStatus.Failed);
        }
    }
}