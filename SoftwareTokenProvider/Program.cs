using System;
using System.Threading;
using MultiFactorAuthentication.Abstractions;
using TinyIoC;

namespace SoftwareTokenProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            TinyIoCContainer.Current.AutoRegister(DuplicateImplementationActions.RegisterSingle);

            Console.Clear();

            Console.WriteLine("TOTP Token Provider");
            Console.WriteLine();

            Console.WriteLine("Provide QR Code URL");

            var authData = Console.ReadLine().ToAuthData();
            Console.WriteLine();

            Console.WriteLine($"Scheme:      {authData.Scheme}");
            Console.WriteLine($"Host:        {authData.Host}");
            Console.WriteLine();

            Console.WriteLine($"Application: {authData.Application}");
            Console.WriteLine($"User:        {authData.User}");
            Console.WriteLine();

            Console.WriteLine($"Issuer:      {authData.Issuer}");
            Console.WriteLine($"Secret:      {authData.Secret}");
            Console.WriteLine($"Digits:      {authData.Digits}");
            Console.WriteLine();

            Console.WriteLine("Generating tokens ...");
            Console.WriteLine();

            var tokenProvider = TinyIoCContainer.Current.Resolve<ITotpTokenProvider>();

            while (true)
            {
                var token = tokenProvider.ComputeToken(authData.Secret);
                var timeRemaining = tokenProvider.GetRemainingSecondsInCurrentInterval();

                for (var i = timeRemaining; i > 0; i--)
                {
                    Console.BackgroundColor = i <= 5 ? ConsoleColor.DarkRed : ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(token);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("\r Token expires in {0} seconds  ", i);
                    Thread.Sleep(1000);

                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            }
        }
    }
}
