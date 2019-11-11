using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Regex_example
{
    public class Pharser
    {

        public static List<Data> lData = new List<Data>();

        public void RunPharser()
        {
            Pharse oPharse = new Pharse();
            oPharse.ReadFromFile("Plik");           
        }

    }
}
