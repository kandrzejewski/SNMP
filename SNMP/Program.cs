﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Regex_example
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadFile oReadFile = new ReadFile();
            oReadFile.ReadFromFile(@"Plik");
        }
    }
}
