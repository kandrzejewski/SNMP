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
            ObjectType oObjectType = new ObjectType();
            EncodingValidator oEncodingValidator = new EncodingValidator();
            EncoderData oEncoderData = new EncoderData();

            oPharser.RunPharser("RFC1213-MIB");
            oDataHub.GenerateTree();
            
            while(sExit != "Q" & sExit != "q")
            {

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(30, Console.CursorTop++);
                Console.WriteLine("------------------------------------------------------------");
                Console.SetCursorPosition(30, Console.CursorTop++);
                Console.WriteLine("                         Agent SNMP                         ");
                Console.SetCursorPosition(30, Console.CursorTop++); Console.CursorTop++;
                Console.WriteLine("----------------------------MENU----------------------------\n");
                Console.SetCursorPosition(30, Console.CursorTop++); Console.CursorTop++;
                Console.WriteLine("T) Print tree of Object Types");
                Console.SetCursorPosition(30, Console.CursorTop++); Console.CursorTop++;
                Console.WriteLine("F) Find Object Type by OID number");
                Console.SetCursorPosition(30, Console.CursorTop++); Console.CursorTop++;
                Console.WriteLine("D) Print List of Data Types");
                Console.SetCursorPosition(30, Console.CursorTop++); Console.CursorTop++;
                Console.WriteLine("V) Validate the data to Encode | OID");
                Console.SetCursorPosition(30, Console.CursorTop++); Console.CursorTop++;
                Console.WriteLine("W) Validate the data to Encode | Any Data Type");
                Console.SetCursorPosition(30, Console.CursorTop++); Console.CursorTop++;
                Console.WriteLine("Q) Quit Program");
                Console.SetCursorPosition(30, Console.CursorTop++); Console.CursorTop++;
                
                sExit = Console.ReadLine();

                if (sExit == "T" || sExit == "t")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    oDataHub.PrintObjectTree();
                    Console.ReadKey();
                }
                if (sExit == "F" || sExit == "f")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 3);
                    oObjectType = oDataHub.FindByOID();
                    if (oObjectType != null)
                    {   
                        Console.Write("\n┌");
                        for (int i = 0; i < oDataHub.sOID.Count(); i++)
                        {
                            Console.Write("─");
                        }
                        Console.WriteLine("┐");
                        Console.Write("│");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(oDataHub.sOID);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("│");
                        Console.Write("└┬");
                        for (int i = 0; i < oDataHub.sOID.Count() - 1; i++)
                        {
                            Console.Write("─");
                        }
                        Console.WriteLine("┘");
                        oObjectType.PresentData(1);
                        Console.ReadKey();
                    }
                }
                if (sExit == "D" || sExit == "d")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    oDataHub.PrintDataTypes();
                    Console.ReadKey();
                }
                if (sExit == "V" || sExit == "v")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    oObjectType = oDataHub.FindByOID();
                    if (oObjectType != null)
                    {
                        oEncoderData = oEncodingValidator.Validate("\n\n Data Type of object " + oObjectType.Name, oObjectType.Syntax, 0);
                        oEncoderData.PresentData();
                    }

                    Console.ReadKey();
                }
                if (sExit == "W" || sExit == "w")
                {
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    oEncoderData = oEncodingValidator.ValidateAnyDataType();
                    if (oEncoderData != null)
                    {
                        oEncoderData.PresentData();
                    }

                    Console.ReadKey();
                }
            }
            Console.SetCursorPosition(30, Console.CursorTop-1);
        }
    }
}
