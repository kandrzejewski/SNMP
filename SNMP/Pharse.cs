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
                                        @"(?<email>[a-z][a-z0-9_.-]*@[a-z0-9]*\.[a-z]{2,3})\s*{[0-9]}\s*Country\s*-\s*" + 
                                        @"(?<country>\w*)\s*{[1-9]}\s*title:\s*" + 
                                        @"(?<title>.*?)\s*{[1-9]}\s*name:\s*" + 
                                        @"(?<name>\w*\s*\w*)\s*{[1-9]}", options);
            Regex regNewFile = new Regex(@"openfile\s*-\s*"+"\""+ @"(?<name>.*?)" + "\""+@"\s*");

            MatchCollection matches = regEmail.Matches(_text);
            MatchCollection matchesimport = regNewFile.Matches(_text);

            foreach (Match _match in matches)
            {
                Data oData = new Data();
                for(int _iGroupNumber = 1; _iGroupNumber < _match.Groups.Count; _iGroupNumber++)
                {
                    //Console.WriteLine("Group {0}: {1}", regEmail.GroupNameFromNumber(_iGroupNumber), _match.Groups[_iGroupNumber].Value);
                    switch (_iGroupNumber)
                    {
                        case 1:
                            {
                                oData.id = Int32.Parse(_match.Groups[_iGroupNumber].Value);
                                continue;
                            }
                        case 2:
                            {
                                oData.email = _match.Groups[_iGroupNumber].Value;
                                continue;
                            }
                        case 3:
                            {
                                oData.country = _match.Groups[_iGroupNumber].Value;
                                continue;
                            }
                        case 4:
                            {
                                oData.title = _match.Groups[_iGroupNumber].Value;
                                continue;
                            }
                        case 5:
                            {
                                oData.name = _match.Groups[_iGroupNumber].Value;
                                continue;
                            }
                    }
                }
                oData.WriteData();
            }

            if (regNewFile.IsMatch(_text))
            {
                Console.WriteLine(@"Wystąpiło wywołanie 2 pliku");
                Match _match = matchesimport[0];
                Console.WriteLine(_match.Groups[1].Value);
                ReadFile NewFile = new ReadFile();
                NewFile.ReadFromFile(_match.Groups[1].Value);
            }
        }
    }
}
