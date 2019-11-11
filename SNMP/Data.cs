using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex_example
{
    public class Data
    {
        public int id {get; set;}
        public string email { get; set; }
        public string country { get; set; }
        public string title { get; set; }
        public string name { get; set; }

        /*public Data(int _id, string _email, string _country, string _title, string _name)
        {
            this.id = _id;
            this.email = _email;
            this.country = _country;
            this.title = _title;
            this.name = _name;
        }*/

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
