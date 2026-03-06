using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.Education
{
    public abstract class Question
    {
        public string Content { get; protected set; }

        public abstract bool Validate(string userAnswer);

        public abstract string GetCorrectAnswer();
    }
}
