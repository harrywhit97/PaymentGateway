using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Domain.Clients;
using PaymentGateway.Domain.Models;
using PaymentGateway.Domain.Pipeline;
using PaymentGateway.Domain.Repository;
using PaymentGateway.Domain.Requests;
using System.Reflection;

namespace PaymentGateway.Domain
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureDomain(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<ProcessPaymentRequest>();
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            RegisterValidators(services);

            services.AddTransient<ICkoClient, MockCkoClient>();
            services.AddSingleton<IRepository<Payment>, PaymentRepository>();
            services.AddSingleton<IRepository<Merchant>, MerchantRepository>();
            return services;
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            var validators = typeof(ProcessPaymentRequest)
                               .Assembly
                               .GetTypes()
                               .Where(IsValidator);
            foreach (var validator in validators)
            {
                services.AddTransient(validator);
            }
        }

        private static bool IsValidator(Type t)
        {
            return t.GetInterfaces().Any(x => x.Name == typeof(IValidator<>).Name)
                    && t.IsClass
                    && !t.IsAbstract
                    && t.IsPublic;
        }
    }
}
