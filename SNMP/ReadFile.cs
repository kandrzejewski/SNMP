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

        public void ReadFromFile(string sFileName, string sPath) {
            try
            {
                var sLines = File.ReadLines(sPath + @"\"+ sFileName + @".txt", Encoding.UTF8);
                foreach (var line in sLines)
                {
                    sLine = line;
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
