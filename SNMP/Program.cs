using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    public class Program
    {

        static void Main(string[] args)
        {
            string sExit = "";
            Pharser oPharser = new Pharser();
            DataHub oDataHub = new DataHub();

            oPharser.RunPharser("RFC1213-MIB");
            oDataHub.GenerateTree();
            
            while(sExit != "Q" & sExit != "q")
            {
                
                ConsoleConfig();
                sExit = Console.ReadLine();
                if (sExit == "T" || sExit == "t")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    oDataHub.WriteTree();
                    Console.ReadKey();
                }
                if (sExit == "F" || sExit == "f")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    ObjectType oOT = new ObjectType();
                    oOT = oDataHub.FindByOID();
                    oOT.PresentData(0);
                    Console.ReadKey();
                }
                if (sExit == "D" || sExit == "d")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    oDataHub.WriteTypes();
                    Console.ReadKey();
                }  
            }      
        }

        private static void ConsoleConfig()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(30, 1);
            Console.WriteLine("------------------------------------------------------------");
            Console.SetCursorPosition(30, 2);
            Console.WriteLine("                         Agent SNMP                         ");
            Console.SetCursorPosition(30, 4);
            Console.WriteLine("----------------------------MENU----------------------------\n");
            Console.SetCursorPosition(30, 6);
            Console.WriteLine("T) Print tree of Object Types");
            Console.SetCursorPosition(30, 8);
            Console.WriteLine("F) Find Object Type by OID number");
            Console.SetCursorPosition(30, 10);
            Console.WriteLine("D) Print List of Data Types");
            Console.SetCursorPosition(30, 12);
            Console.WriteLine("Q) Quit Program");
            Console.SetCursorPosition(30, 15);
        }
    }
}
