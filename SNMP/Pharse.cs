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
            RegexOptions options = RegexOptions.Singleline & RegexOptions.Compiled;
            Regex regEmail = new Regex(@"{Person:\s*(?<Person>[1-9])}\s*e-mail:\s*" + 
                                        @"(?<email>[a-z][a-z0-9_.-kam-]*@[a-z0-9]*\.[a-z]{2,3})\s*{[0-9]}\s*Country\s*-\s*" + 
                                        @"(?<country>\w*)\s*{[1-9]}\s*title:\s*" + 
                                        @"(?<title>.*?)\s*{[1-9]}\s*name:\s*" + 
                                        @"(?<name>\w*\s*\w*)\s*{[1-9]}", options);
            Regex regNewFile = new Regex(@"Plik2");

            MatchCollection matches = regEmail.Matches(_text);
            foreach (Match _match in matches)
            {
                for(int _iGroupNumber = 1; _iGroupNumber < _match.Groups.Count; _iGroupNumber++)
                {
                    Console.WriteLine("Group {0}: {1}", regEmail.GroupNameFromNumber(_iGroupNumber), _match.Groups[_iGroupNumber].Value);
                }
            }

            if (regNewFile.IsMatch(_text))
            {
                Console.WriteLine(@"Wystąpiło wywołanie 2 pliku");
                ReadFile NewFile = new ReadFile();
                NewFile.ReadFromFile(_text);
            }
        }
    }
}
