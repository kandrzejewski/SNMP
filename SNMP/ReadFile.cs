using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Regex_example
{
    public class ReadFile
    {
        public string sLine;

        public void ReadFromFile(string sFileName) {
            try
            {
                var sLines = File.ReadAllText(@"../data/" + sFileName + @".txt", Encoding.UTF8);
                Console.WriteLine("Odczytano plik {0}!", sFileName);
                //Console.WriteLine(sLines);
                Pharse PharseLine = new Pharse();
                PharseLine.PharseText(sLines);
            }
            catch (IOException e)
            {
                Console.WriteLine("Nie można otworzyć pliku:");
                Console.WriteLine(e.Message);
            }
           
        }
    }
}
