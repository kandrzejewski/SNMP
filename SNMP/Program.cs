using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex_example
{
    public class Program
    {
        public static List<Data> DataList;

        static void Main(string[] args)
        {
            Pharser oPharser = new Pharser();
            DataHub oDataHub = new DataHub();

            oPharser.RunPharser();
            oDataHub.GenerateTree();
            oDataHub.WriteTree();

            /*foreach (Data _Data in DataHub.lData)
            {
                Console.WriteLine("-------------------");
                Console.WriteLine(_Data.id);
                Console.WriteLine(_Data.parrentId);
                Console.WriteLine(_Data.email);
                Console.WriteLine(_Data.country);
                Console.WriteLine(_Data.title);
                Console.WriteLine(_Data.name);
                Console.WriteLine("-------------------");
            }*/
            //Console.WriteLine((char)6);
        }
    }
}
