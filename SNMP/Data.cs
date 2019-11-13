using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex_example
{
    public class Data
    {
        public int id { get; set; }
        public int parrentId {get; set;}
        public string email { get; set; }
        public string country { get; set; }
        public string title { get; set; }
        public string name { get; set; }

        public void WriteData(int _iWriteIteration)
        {
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine("------------");
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine(id);
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine(parrentId);
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine(email);
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine(country);
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine(title);
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine(name);
        }
        private void WriteTreeNode(int _iWriteIteration)
        {
            for (int i = 0; i < _iWriteIteration; i++)
            {
                //if (i == _iWriteIteration-1)
                //{
                    Console.Write("     |");
                //}
                //else
                //{
                //    Console.Write("      ");
                //}

            }
        }
    }
}
