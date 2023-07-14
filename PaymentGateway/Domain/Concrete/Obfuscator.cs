namespace PaymentGateway.Domain.Concrete
{
    public static class Obfuscator
    {
        public static string Obfuscate(string intput)
            => new('*', intput.Length);
    }
}
