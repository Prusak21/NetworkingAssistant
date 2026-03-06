using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using AsystentSieciowca.Education; 
using AsystentSieciowca.Interfaces;
namespace AsystentSieciowca.Modules
{
    public class EducationModule : IEducationModule
    {
        private readonly QuizRepository _repository;
        private readonly QuizSettings _settings;
        private readonly List<QuizAttempt> _history;

        public EducationModule()
        {
            _repository = new QuizRepository();
            _settings = new QuizSettings();
            _history = new List<QuizAttempt>();
        }

        public string GetModuleName() => "Centrum Edukacji";

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== CENTRUM EDUKACJI SIECIOWEJ ===");
                Console.WriteLine($"[Ustawienia]: Pytania: {_settings.QuestionCount} | Czas: {_settings.TimeLimitMinutes} min");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("[1] Quiz Teoretyczny (Pytania z pliku txt)");
                Console.WriteLine("[2] Quiz Praktyczny (Obliczanie podsieci)");
                Console.WriteLine("[3] Ustawienia Quizu");
                Console.WriteLine("[4] Historia Wyników");
                Console.WriteLine("[Q] Powrót do menu głównego");
                Console.Write("\nTwój wybór > ");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Q: return;
                   
                    case ConsoleKey.D1: StartQuiz(practicalMode: false); break;
                    case ConsoleKey.D2: StartQuiz(practicalMode: true); break;
                    case ConsoleKey.D3: ConfigureSettings(); break;
                    case ConsoleKey.D4: ShowHistory(); break;
                }
            }
        }
        private void StartQuiz(bool practicalMode)
        {
            //AGREGACJA (Zbieranie pytań do listy)
            List<Question> questions = new List<Question>();

            if (practicalMode)
            {
                // Generowanie zadań matematycznych
                for (int i = 0; i < _settings.QuestionCount; i++)
                {
                  
                    if (i % 2 == 0) questions.Add(new HostCountQuestion());
                    else questions.Add(new BroadcastQuestion());
                }
            }
            else
            {
                // Ładowanie z pliku
                try
                {
                    var loaded = _repository.LoadQuestions();
                    if (loaded.Count == 0)
                    {
                        Console.WriteLine("Błąd: Plik z pytaniami jest pusty!");
                        Console.ReadKey();
                        return;
                    }

                  
                    Random rng = new Random();
                    questions = loaded.OrderBy(x => rng.Next()).Take(_settings.QuestionCount).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd pliku: {ex.Message}");
                    Console.ReadKey();
                    return;
                }
            }

           
            var attempt = new QuizAttempt
            {
                Date = DateTime.Now,
                TotalQuestions = questions.Count,
                Details = new List<QuestionResult>()
            };

            Console.Clear();
            string modeName = practicalMode ? "PRAKTYCZNY" : "TEORETYCZNY";
            Console.WriteLine($"=== QUIZ {modeName} ===");
            Console.WriteLine($"Masz {_settings.TimeLimitMinutes} minut na {questions.Count} pytań.");
            Console.WriteLine("Naciśnij ENTER, aby startować...");
            Console.ReadLine();

            Stopwatch sw = Stopwatch.StartNew();
            TimeSpan timeLimit = TimeSpan.FromMinutes(_settings.TimeLimitMinutes);

            int currentIdx = 1;
            foreach (var q in questions)
            {
                // Sprawdzenie czasu
                if (sw.Elapsed > timeLimit)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n!!! KONIEC CZASU !!!");
                    Console.ResetColor();
                    break;
                }

                Console.Clear();
                Console.WriteLine($"Czas: {sw.Elapsed:mm\\:ss} / {timeLimit:mm\\:ss}");
                Console.WriteLine($"PYTANIE {currentIdx}/{questions.Count}:");
                Console.WriteLine(new string('-', 40));

              
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(q.Content);
                Console.ResetColor();

                Console.WriteLine(new string('-', 40));
                Console.Write("Odpowiedź: ");

                string input = Console.ReadLine();

        
                bool isCorrect = q.Validate(input);

                attempt.Details.Add(new QuestionResult
                {
                    Question = q.Content,
                    UserAnswer = input,
                    CorrectAnswer = q.GetCorrectAnswer(),
                    IsCorrect = isCorrect
                });

                if (isCorrect)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(">> DOBRZE!");
                    attempt.Score++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($">> ŹLE! Poprawna: {q.GetCorrectAnswer()}");
                }
                Console.ResetColor();

                System.Threading.Thread.Sleep(1000);
                currentIdx++;
            }

            sw.Stop();
            attempt.TimeTaken = sw.Elapsed;
            _history.Add(attempt);

            ShowSummary(attempt);
        }
        private void ConfigureSettings()
        {
            Console.Clear();
            Console.WriteLine("--- KONFIGURACJA QUIZU ---");

            Console.Write($"Podaj liczbę pytań (aktualnie {_settings.QuestionCount}): ");
            if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
                _settings.QuestionCount = count;

            Console.Write($"Podaj limit czasu w minutach (aktualnie {_settings.TimeLimitMinutes}): ");
            if (int.TryParse(Console.ReadLine(), out int time) && time > 0)
                _settings.TimeLimitMinutes = time;

            Console.WriteLine("\nZapisano! Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }
     
        
        private void ShowSummary(QuizAttempt attempt)
        {
            Console.Clear();
            Console.WriteLine("=== PODSUMOWANIE ===");
            Console.WriteLine($"Wynik: {attempt.Score} / {attempt.TotalQuestions}");
            Console.WriteLine($"Czas:  {attempt.TimeTaken:mm\\:ss}");

            double percent = (double)attempt.Score / attempt.TotalQuestions * 100;
            Console.WriteLine($"Skuteczność: {percent:F1}%");

            if (percent >= 90) Console.ForegroundColor = ConsoleColor.Green;
            else if (percent >= 50) Console.ForegroundColor = ConsoleColor.Yellow;
            else Console.ForegroundColor = ConsoleColor.Red;

            if (percent >= 90) Console.WriteLine("Ocena: CELUJĄCY!");
            else if (percent >= 50) Console.WriteLine("Ocena: ZALICZONE.");
            else Console.WriteLine("Ocena: OBLANE.");

            Console.ResetColor();
            Console.WriteLine("\n[1] Pokaż szczegółowe błędy");
            Console.WriteLine("[Enter] Zakończ");

            if (Console.ReadKey().Key == ConsoleKey.D1)
            {
                Console.WriteLine("\n--- DETALE ---");
                foreach (var d in attempt.Details)
                {
                    if (!d.IsCorrect)
                    {
                        Console.WriteLine($"P: {d.Question}");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Twoja: {d.UserAnswer}");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Poprawna: {d.CorrectAnswer}");
                        Console.ResetColor();
                        Console.WriteLine("-");
                    }
                }
                Console.ReadKey();
            }
        }

        private void ShowHistory()
        {
            Console.Clear();
            Console.WriteLine("=== HISTORIA GIER ===");
            if (_history.Count == 0)
            {
                Console.WriteLine("Brak wyników w tej sesji.");
            }
            else
            {
                for (int i = _history.Count - 1; i >= 0; i--)
                {
                    var h = _history[i];
                    Console.WriteLine($"{h.Date:HH:mm} | Wynik: {h.Score}/{h.TotalQuestions} | Czas: {h.TimeTaken:mm\\:ss}");
                }
            }
            Console.WriteLine("\nNaciśnij dowolny klawisz...");
            Console.ReadKey();
        }
    }
}
