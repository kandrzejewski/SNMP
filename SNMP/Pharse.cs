using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Regex_example
{
    class Pharse
    {
        public void PharseText(string _text)
        {
            Regex regEmail = new Regex(@"(?<name>^[a-z][a-z0-9_.-kam-]*)@(?<domain>[a-z0-9]*\.[a-z]{2,3}$)");
            Regex regNewFile = new Regex(@"Plik2");

            MatchCollection matches = regEmail.Matches(_text);
            foreach (Match _match in matches)
            {
                Console.WriteLine("Adres e-mail: " + _match);
                Console.Write(regEmail.GroupNameFromNumber(1) + ": ");
                Console.WriteLine(_match.Groups[1].Value);
                Console.Write(regEmail.GroupNameFromNumber(2) + ": ");
                Console.WriteLine(_match.Groups[2].Value);
            }

            if (regNewFile.IsMatch(_text))
            {
                Console.WriteLine(@"Wystąpiło wywołanie 2 pliku");
                ReadFile NewFile = new ReadFile();
                NewFile.ReadFromFile(_text);
            }
            // Console.WriteLine(_text);
        }
    }
}
