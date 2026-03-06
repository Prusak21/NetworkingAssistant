using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.Education
{
    public class QuizRepository
    {
        private readonly string _filePath;

        public QuizRepository(string fileName = "quiz_db.txt")
        { 
            if (!File.Exists(_filePath))
            {
                _filePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, fileName);
          }
        }

        public List<Question> LoadQuestions()
        {
            var questions = new List<Question>();

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Nie znaleziono pliku z pytaniami: {_filePath}");
            }

            var lines = File.ReadAllLines(_filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var parts = line.Split('|');

                if (parts.Length == 2)
                {
                    questions.Add(new TextQuestion(parts[0], parts[1]));
                }
            }


            return questions;
        }
    }
}
