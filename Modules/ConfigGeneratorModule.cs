using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsystentSieciowca.ConfigBuilder;
using AsystentSieciowca.Core;
using AsystentSieciowca.Interfaces;

namespace AsystentSieciowca.Modules
{
    public class ConfigGeneratorModule : IConfigGeneratorModule
    {
        
        private IConfigBuilder _builder;

        public string GetModuleName() => "Generator Cisco IOS";

        public void Run()
        {
            _builder = new CiscoBuilder();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== GENERATOR KONFIGURACJI ROUTERA CISCO ===");
                Console.WriteLine("Buduj swój plik konfiguracyjny krok po kroku.");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("[1] Ustawienia Systemowe (Hostname, Banner)");
                Console.WriteLine("[2] Bezpieczeństwo (Hasła, SSH, User)");
                Console.WriteLine("[3] Dodaj Interfejs (IP Address)");
                Console.WriteLine("[4] Dodaj Serwer DHCP");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("[G] GENERUJ I ZAPISZ PLIK");
                Console.WriteLine("[Q] Wyjdź (Anuluj)");

                Console.Write("\nWybór > ");
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.D1: StepSystem(); break;
                    case ConsoleKey.D2: StepSecurity(); break;
                    case ConsoleKey.D3: StepInterface(); break;
                    case ConsoleKey.D4: StepDhcp(); break;
                    case ConsoleKey.G: GenerateAndSave(); return;
                    case ConsoleKey.Q: return;
                }
            }
        }

        private void StepSystem()
        {
            Console.Clear();
            Console.Write("Podaj Hostname routera: ");
            string host = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(host))
                _builder.SetHostname(host);

            Console.Write("Treść bannera powitalnego (MOTD): ");
            string banner = Console.ReadLine();
            _builder.SetBanner(banner);

            Console.WriteLine("Wyłączono DNS Lookup i poprawiono logowanie konsoli.");
            _builder.DisableDnsLookup();
            _builder.ConfigureConsoleLogging();
            Console.ReadKey();
        }

        private void StepSecurity()
        {
            Console.Clear();
            Console.Write("Hasło uprzywilejowane (Enable Secret): ");
            string secret = Console.ReadLine();
            _builder.SetEnableSecret(secret);
            _builder.EnablePasswordEncryption();

            Console.Write("Nazwa administratora: ");
            string user = Console.ReadLine();
            Console.Write("Hasło administratora: ");
            string pass = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(user))
                _builder.CreateAdminUser(user, pass);

            Console.WriteLine("Włączanie SSH v2...");
            _builder.ConfigureSshAccess();
            Console.ReadKey();
        }

        private void StepInterface()
        {
            Console.Clear();
            Console.WriteLine("--- DODAWANIE INTERFEJSU ---");
            Console.Write("Nazwa (np. GigabitEthernet0/0): ");
            string name = Console.ReadLine();

            // walidator wlasny
            string ip = GetValidIp("Adres IP: ");
            string mask = GetValidIp("Maska podsieci: ");

            Console.Write("Opis (Description): ");
            string desc = Console.ReadLine();

            _builder.AddInterface(name, ip, mask, desc);
            Console.WriteLine("Dodano interfejs.");
            Console.ReadKey();
        }

        private void StepDhcp()
        {
            Console.Clear();
            Console.WriteLine("--- KONFIGURACJA DHCP ---");
            Console.Write("Nazwa puli (np. LAN_POOL): ");
            string pool = Console.ReadLine();

            string net = GetValidIp("Adres sieci: ");
            string mask = GetValidIp("Maska sieci: ");
            string gw = GetValidIp("Brama domyślna: ");

            _builder.AddDhcpPool(pool, net, mask, gw);
            Console.WriteLine("Dodano DHCP.");
            Console.ReadKey();
        }

        private void GenerateAndSave()
        {
            Console.Clear();
            var config = _builder.Build();

            Console.WriteLine("--- PODGLĄD PLIKU ---");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(config.GetContent());
            Console.ResetColor();
            Console.WriteLine("---------------------");

            Console.Write("Podaj nazwę pliku (np. router1.txt): ");
            string filename = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(filename)) filename = "router_config.txt";

            config.SaveToFile(filename);

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey();
        }

        private string GetValidIp(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                var result = AdvancedIPValidator.ValidateIPv4(input);
                if (result.IsValid) return input;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.ErrorMessage);
                Console.ResetColor();
            }
        }
    }
}
