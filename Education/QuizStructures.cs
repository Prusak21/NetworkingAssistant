using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.Education
{
    public class QuizSettings
    {
        public int QuestionCount { get; set; } = 10;      
        public int TimeLimitMinutes { get; set; } = 5;    
    }

    public class QuestionResult
    {
        public string Question { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class QuizAttempt
    {
        public DateTime Date { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public List<QuestionResult> Details { get; set; } = new List<QuestionResult>();
    }
}
