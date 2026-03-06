using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.Modules
{
    using AsystentSieciowca.Core;
    using AsystentSieciowca.Calculator;
    using AsystentSieciowca.Interfaces;
    using System.Net;

    public class SubnetCalculatorModule : ISubnetCalculatorModule
    {
        private readonly IPv4Engine _v4Engine;

        public SubnetCalculatorModule()
        {
            _v4Engine = new IPv4Engine();
        }

        public string GetModuleName() => "Kalkulator Sieciowy";

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== KALKULATOR SIECIOWY ===");
                Console.WriteLine("[1] IPv4 VLSM");
                Console.WriteLine("[2] IPv6 Analiza");
                Console.WriteLine("[Q] Powrót do menu głównego");
                Console.Write("\nWybór > ");

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Q) return;

                if (key == ConsoleKey.D1) RunIPv4Scenario();
                else if (key == ConsoleKey.D2) RunIPv6Scenario();
            }
        }
        private void RunIPv4Scenario()
        {
            Console.Clear();
            Console.WriteLine("--- KREATOR VLSM (IPv4) ---");
            Console.WriteLine("System zaprojektuje optymalny podział sieci.");

            string ip = GetValidInputIPv4("Podaj adres sieci głównej (np. 192.168.0.0): ");

            int cidr = GetValidInt("Podaj maskę CIDR (np. 24): ",
                                   val => val >= 0 && val <= 30);

            List<int> requirements = GetRequirementsList();

            try
            {
                Console.WriteLine("\n[OBLICZANIE] Trwa przetwarzanie algorytmem VLSM...");
                var results = _v4Engine.CalculateVLSM(ip, cidr, requirements);

                DisplayResults(results);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[BŁĄD KRYTYCZNY SILNIKA]: {ex.Message}");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        // obsluga komunikatow bledow z walidatora
        private string GetValidInputIPv4(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                var result = AdvancedIPValidator.ValidateIPv4(input);

                if (result.IsValid)
                {
                    return input;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[BŁĄD]: {result.ErrorMessage}");
                    Console.ResetColor();
                }
            }
        }

        private int GetValidInt(string prompt, Func<int, bool> validator)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && validator(value))
                {
                    return value;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Wartość niepoprawna (zły format lub poza zakresem). Spróbuj ponownie.");
                Console.ResetColor();
            }
        }

        // lista hostow
        private List<int> GetRequirementsList()
        {
            while (true)
            {
                Console.WriteLine("\nPodaj liczby hostów dla podsieci oddzielone spacją (np. '600 20 100'):");
                Console.Write("> ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Musisz podać przynajmniej jedną wartość.");
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    // dzielenie po spacji i oddziwlanie liczb
                    var list = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => int.Parse(x))
                                    .ToList();

                    if (list.Count > 0) return list;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[BŁĄD]: Wpisz tylko liczby całkowite oddzielone spacją.");
                    Console.ResetColor();
                }
            }
        }

        private void RunIPv6Scenario()
        {
            Console.Clear();
            Console.WriteLine("--- KALKULATOR PODSIECI IPv6 ---");
            Console.WriteLine("Dzielenie dużego prefixu (np. /48) na mniejsze (np. /64)");

            string ip = GetValidInputIPv6("Podaj adres sieci IPv6 (np. 2001:db8:acad::): ");

            int currentCidr = int.Parse(GetValidInputGeneric("Podaj obecny CIDR (np. 48): ",
                s => int.TryParse(s, out int v) && v >= 0 && v < 128));

            int targetCidr = int.Parse(GetValidInputGeneric($"Podaj docelowy CIDR (musi być > {currentCidr}, np. 64): ",
                s => int.TryParse(s, out int v) && v > currentCidr && v <= 128));

            try
            {
                var v6Engine = new AsystentSieciowca.Calculator.IPv6Engine();

                Console.Write("Ile podsieci wyświetlić? (domyślnie 8): ");
                string limitStr = Console.ReadLine();
                int limit = int.TryParse(limitStr, out int l) ? l : 8;

                var results = v6Engine.GenerateSubnets(ip, currentCidr, targetCidr, limit);

                Console.WriteLine("\n=== WYNIKI PODZIAŁU IPv6 ===");
                Console.WriteLine($"{"Lp.",-4} | {"Prefix Podsieci",-40} | {"CIDR",-5}");
                Console.WriteLine(new string('-', 60));

                int counter = 1;
                foreach (var r in results)
                {
                    Console.WriteLine($"{counter++,-4} | {r.NetworkAddress,-40} | {r.SubnetMask}");
                }

                Console.WriteLine(new string('-', 60));
                Console.WriteLine("(Wyświetlono tylko początkowe podsieci)");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Błąd obliczeń IPv6: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz...");
            Console.ReadKey();
        }
        private string GetValidInputIPv6(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                var result = AdvancedIPValidator.ValidateIPv6(input);

                if (result.IsValid) return input;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[BŁĄD]: {result.ErrorMessage}");
                Console.ResetColor();
            }
        }
        private string GetValidInputGeneric(string prompt, Func<string, bool> validator)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (validator(input)) return input;
                Console.WriteLine("Błędna wartość.");
            }
        }

        private void DisplayResults(List<SubnetResult> results)
        {
            Console.WriteLine("\n=== WYNIKI VLSM ===");
            Console.WriteLine($"{"Wymagane",-10} | {"Adres Sieci",-16} | {"CIDR",-5} | {"Maska",-15} | {"Zakres Hostów"}");
            Console.WriteLine(new string('-', 85));

            foreach (var r in results)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{r.HostsCount,-10} | ");
                Console.ResetColor();
                Console.Write($"{r.NetworkAddress,-16} | /{r.Cidr,-4} | {r.SubnetMask,-15} | {r.FirstHost} - {r.LastHost}");
                Console.WriteLine();
            }
            Console.WriteLine(new string('-', 85));
            Console.WriteLine("Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }
    }
}
