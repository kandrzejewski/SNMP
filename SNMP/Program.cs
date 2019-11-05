using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;

namespace Regex_example
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex regEmail = new Regex(@"^[a-z][a-z0-9_.-kam-]*@[a-z0-9]*\.[a-z]{2,3}$");
            ReadFile ReadMIB = new ReadFile();
            string napis;
            ReadMIB.ReadFromFile(@"Plik", @"..\data");
            napis = ReadMIB.sLine;
            Console.WriteLine(
                String.Format("{0} podany adres to {1}poprawny adres email", napis,
                                regEmail.IsMatch(napis) ? "" : "nie"));
        }
    }
}
