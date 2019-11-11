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
            Regex regEmail = new Regex(@"{Person:\s*(?<person>[1-9])}\s*e-mail:\s*" + 
                                       @"(?<email>[a-z][a-z0-9_.-]*@[a-z0-9]*\.[a-z]{2,3})\s*{[0-9]}\s*Country\s*-\s*" + 
                                       @"(?<country>\w*)\s*{[1-9]}\s*title:\s*" + 
                                       @"(?<title>.*?)\s*{[1-9]}\s*name:\s*" +
                                       @"(?<name>\w*\s*\w*)\s*{[1-9]}.*?" +
                                       @".*?|openfile\s*-\s*" + "\"" + @"(?<filename>.*?)" + "\"" + @"\s*", options);

            MatchCollection matches = regEmail.Matches(_text);

            foreach (Match _match in matches)
            {
                Data oData = new Data();
                for(int _iGroupNumber = 1; _iGroupNumber < _match.Groups.Count; _iGroupNumber++)
                {
                    if (_match.Groups[_iGroupNumber].Value != "")
                    {
                        //Console.WriteLine("Group {0}: {1}", regEmail.GroupNameFromNumber(_iGroupNumber), _match.Groups[_iGroupNumber].Value);
                        switch (regEmail.GroupNameFromNumber(_iGroupNumber))
                        {
                            case "person":
                                {
                                    oData.id = Int32.Parse(_match.Groups[_iGroupNumber].Value);
                                    continue;
                                }
                            case "email":
                                {
                                    oData.email = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "country":
                                {
                                    oData.country = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "title":
                                {
                                    oData.title = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "name":
                                {
                                    oData.name = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "filename":
                                {
                                    OpenNewFile(_match.Groups[_iGroupNumber].Value);
                                    continue;
                                }
                        }
                    }                    
                }
                oData.WriteData();
            }
        }

        private void OpenNewFile(string _filename)
        {
            Console.WriteLine("");
            Console.WriteLine(@"..............................");
            Console.Write(@"Wystąpiło wywołanie pliku: ");
            Console.WriteLine(_filename);
            ReadFile NewFile = new ReadFile();
            NewFile.ReadFromFile(_filename);
        }
    }
}
