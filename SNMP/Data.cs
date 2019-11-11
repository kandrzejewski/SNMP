using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex_example
{
    class Data
    {
        public int id;
        public string email;
        public string country;
        public string title;
        public string name;

        public void WriteData()
        {
            Console.WriteLine("------------");
            Console.WriteLine(id);
            Console.WriteLine(email);
            Console.WriteLine(country);
            Console.WriteLine(title);
            Console.WriteLine(name);
            Console.WriteLine("------------");
        }

        //public Data oSubData = new Data();
    }
}
