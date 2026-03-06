using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace AsystentSieciowca.Core
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string ErrorMessage { get; }

        private ValidationResult(bool isValid, string error)
        {
            IsValid = isValid;
            ErrorMessage = error;
        }

        public static ValidationResult Success() => new ValidationResult(true, "OK");
        public static ValidationResult Failure(string error) => new ValidationResult(false, error);
    }

    public static class AdvancedIPValidator
    {
        public static ValidationResult ValidateIPv4(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return ValidationResult.Failure("Adres nie może być pusty.");

            string[] octets = input.Split('.');

            if (octets.Length != 4)
                return ValidationResult.Failure($"Nieprawidłowy format. Oczekiwano 4 oktetów, znaleziono: {octets.Length}.");

            for (int i = 0; i < octets.Length; i++)
            {
                if (!int.TryParse(octets[i], out int value))
                    return ValidationResult.Failure($"Oktet nr {i + 1} ('{octets[i]}') nie jest liczbą.");

                if (value < 0 || value > 255)
                    return ValidationResult.Failure($"Oktet nr {i + 1} ({value}) wykracza poza zakres 0-255.");
            }

            if (!IPAddress.TryParse(input, out _))
                return ValidationResult.Failure("Znaki niedozwolone w adresie IP.");

            return ValidationResult.Success();
        }

        public static ValidationResult ValidateIPv6(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return ValidationResult.Failure("Adres nie może być pusty.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^[0-9a-fA-F:]+$"))
                return ValidationResult.Failure("Adres zawiera niedozwolone znaki (tylko 0-9, A-F i :).");

            if (input.Count(c => c == ':') < 2)
                return ValidationResult.Failure("Adres IPv6 musi zawierać co najmniej dwa dwukropki.");

            if (input.Length > 45)
                return ValidationResult.Failure("Adres jest zbyt długi.");

            if (!IPAddress.TryParse(input, out var ip) || ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
                return ValidationResult.Failure("Niepoprawna struktura adresu IPv6.");

            return ValidationResult.Success();
        }
    }
}
