using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Dtos;
using PaymentGateway.Domain.Concrete;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Domain.Models;
using PaymentGateway.Domain.Requests;

namespace PaymentGateway.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("process")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessPaymentSuccessDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ProcessAsync(ProcessPaymentDto processPayment)
        {

            var request = ToRequest(processPayment);
            var result = await _mediator.Send(request);
            return result.Match(
                ok => Ok(ToSuccessResult(ok)),
                error => ToErrorResult(error.Value)
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Payment))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ProcessAsync(Guid id)
        {
            var request = new GetPaymentRequest(id);

            var result = await _mediator.Send(request);
            return result.Match(
                ok => Ok(ToDto(ok)),
                error => ToErrorResult(error.Value)
            );
        }

        private static PaymentDto ToDto(Payment ok)
            => new()
            {
                Status= ok.Status,
                Id= ok.Id,
                Created = ok.Created,
                Modified = ok.Modified,
                Amount = new()
                {
                    Amount = ok.Amount.Amount,
                    Currency = ok.Amount.Currency,
                },
                Card = new(ok.Card)
            };

        private IActionResult ToErrorResult(Exception value)
            => value switch
            {
                ValidationException validation => BadRequest(validation.Message),
                NotFoundException notFound => NotFound(notFound.Message),
                _ => StatusCode(500, value.Message)
            };

        private static ProcessPaymentSuccessDto ToSuccessResult(Payment payment)
            => new()
            {
                PaymentId = payment.Id
            };

        private static ProcessPaymentRequest ToRequest(ProcessPaymentDto processPayment)
        {
            var amount = new Money()
            {
                Amount = processPayment.Money.Amount,
                Currency = processPayment.Money.Currency,
            };
            var card = new Card()
            {
                CVV = processPayment.Card.CCV,
                ExpirationMonth = processPayment.Card.ExpirationMonth,
                ExpirationYear = processPayment.Card.ExpirationYear,
                Number = processPayment.Card.Number,
            };
            return new ProcessPaymentRequest(amount, card, processPayment.MerchantId);
        }
    }
}
