using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsystentSieciowca.Interfaces;

namespace AsystentSieciowca
{
    public class ApplicationFacade
    {
        // referencje do modulow
    private readonly ISubnetCalculatorModule _calculatorModule;
    private readonly IConfigGeneratorModule _configModule;
    private readonly IEducationModule _educationModule;


        // nie tworzymy modulow nowych tylko je przkazujemy, wstrzykiwanie zależności
        public ApplicationFacade(
        ISubnetCalculatorModule calculator,
        IConfigGeneratorModule config,
        IEducationModule education)
    {
        _calculatorModule = calculator;
        _configModule = config;
        _educationModule = education;
    }
    public void Start()
    {
        ShowIntro();

        bool isRunning = true;
        while (isRunning)
        {
            Console.Clear();
            ShowHeader();
            ShowMainMenu();

            try
            {
                HandleInput(ref isRunning);
            }
            catch (Exception ex)
            {
                ShowError($"Wystąpił nieoczekiwany błąd w menu głównym: {ex.Message}");
            }
        }

        ShowOutro();
    }

    private void HandleInput(ref bool isRunning)
    {
        Console.Write("\n[Twoja decyzja] > ");
        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.D1:
            case ConsoleKey.NumPad1:
                LaunchModule(_calculatorModule);
                break;

            case ConsoleKey.D2:
            case ConsoleKey.NumPad2:
                LaunchModule(_configModule);
                break;

            case ConsoleKey.D3:
            case ConsoleKey.NumPad3:
                LaunchModule(_educationModule);
                break;

            case ConsoleKey.Q:
            case ConsoleKey.Escape:
                isRunning = false;
                break;

            default:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nNieznana opcja. Wybierz 1, 2, 3 lub Q.");
                Console.ResetColor();
                Thread.Sleep(1000);
                break;
        }
    }
    private void LaunchModule(object module)
    {
        if (module == null)
        {
            ShowError("Ten moduł nie został jeszcze zaimplementowany lub załadowany!");
            return;
        }

        try
        {

            if (module is ISubnetCalculatorModule calc)
                calc.Run();
            else if (module is IConfigGeneratorModule conf)
                conf.Run();
            else if (module is IEducationModule edu)
                edu.Run();
        }
        catch (Exception ex)
        {
            ShowError($"Błąd wewnątrz modułu: {ex.Message}");
        }
    }
    private void ShowHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==================================================");
        Console.WriteLine("                  ASYSTENT SIECIOWCA              ");
        Console.WriteLine("     System Projektowania i Edukacji Sieciowej    ");
        Console.WriteLine("==================================================");
        Console.ResetColor();
    }

    private void ShowMainMenu()
    {
        Console.WriteLine("DOSTĘPNE MODUŁY:");
        Console.WriteLine("   [1] Kalkulator Sieciowy (VLSM / FLSM)");
        Console.WriteLine("   [2] Generator Konfiguracji Routera (Cisco)");
        Console.WriteLine("   [3] Centrum Edukacji (Quizy & Teoria)");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("   [Q] Zamknij System");
    }

    private void ShowIntro()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Inicjalizacja systemu...");
        Thread.Sleep(500); 
        Console.WriteLine("Ładowanie modułów...");
        Thread.Sleep(500);
        Console.ResetColor();
    }

    private void ShowOutro()
    {
        Console.Clear();
        Console.WriteLine("Zamykanie sesji... Do widzenia!");
        Thread.Sleep(1000);
    }

    private void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[BŁĄD KRYTYCZNY]: {message}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ResetColor();
        Console.ReadKey();
    }
}

}