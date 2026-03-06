using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AsystentSieciowca.Education
{
    public class TextQuestion : Question
    {
        private readonly string _correctAnswer;

        public TextQuestion(string content, string answer)
        {
            Content = content;
            _correctAnswer = answer;
        }

        public override bool Validate(string userAnswer)
        {
            if (string.IsNullOrWhiteSpace(userAnswer)) return false;
            return userAnswer.Trim().Equals(_correctAnswer, StringComparison.OrdinalIgnoreCase);
        }

        public override string GetCorrectAnswer() => _correctAnswer;

    }
    public class BroadcastQuestion : Question
    {
        private readonly string _calculatedBroadcast;

        public BroadcastQuestion()
        {
            var rnd = new Random();
            int o1 = 192, o2 = 168, o3 = rnd.Next(0, 255), o4 = 0; 
            int cidr = rnd.Next(24, 30); 

            uint ip = (uint)((o1 << 24) | (o2 << 16) | (o3 << 8) | o4);
            uint mask = ~(uint.MaxValue >> cidr);
            uint broadcast = ip | ~mask;

            byte[] b = BitConverter.GetBytes(broadcast);
            if (BitConverter.IsLittleEndian) Array.Reverse(b);
            _calculatedBroadcast = new IPAddress(b).ToString();

            Content = $"Jaki jest adres Broadcast dla sieci {o1}.{o2}.{o3}.{o4}/{cidr}?";
        }

        public override bool Validate(string userAnswer)
        {
            return userAnswer.Trim() == _calculatedBroadcast;
        }

        public override string GetCorrectAnswer() => _calculatedBroadcast;
    }
    public class HostCountQuestion : Question
    {
        private readonly int _correctCount;

        public HostCountQuestion()
        {
            var rnd = new Random();
            int cidr = rnd.Next(20, 31); 

            _correctCount = (int)Math.Pow(2, 32 - cidr) - 2;
            if (_correctCount < 0) _correctCount = 0; 

            Content = $"Ile adresów użytkowych (hostów) mieści się w masce /{cidr}?";
        }

        public override bool Validate(string userAnswer)
        {
            if (int.TryParse(userAnswer, out int val))
            {
                return val == _correctCount;
            }
            return false;
        }

        public override string GetCorrectAnswer() => _correctCount.ToString();
    }
}
