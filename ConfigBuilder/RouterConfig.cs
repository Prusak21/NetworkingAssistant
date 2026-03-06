using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AsystentSieciowca.ConfigBuilder
{
    public class RouterConfig
    {
        private readonly StringBuilder _content = new StringBuilder();

        public void AppendLine(string line)
        {
            _content.AppendLine(line);
        }

        public string GetContent() => _content.ToString();

        public void SaveToFile(string filename)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            File.WriteAllText(path, _content.ToString());
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SUKCES] Plik zapisano w: {path}");
            Console.ResetColor();
        }
    }
}
