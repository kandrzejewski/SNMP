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
                var sLines = File.ReadLines(@"../data/"+ sFileName + @".txt", Encoding.UTF8);
                Pharse PharseLine = new Pharse();
                foreach (var line in sLines)
                {
                    PharseLine.PharseText(line);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Nie można otworzyć pliku:");
                Console.WriteLine(e.Message);
            }
           
        }
    }
}
