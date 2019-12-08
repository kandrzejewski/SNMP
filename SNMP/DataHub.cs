using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SNMP
{
    class DataHub
    {
        public static List<ObjectType> lData;
        public static List<DataType> lDataType;
        public Tree oTree;

        public string sOID;
        private int iWriteIteration = 0;

        public DataHub()
        {
            lData = new List<ObjectType>();
            lDataType = new List<DataType>();
            oTree = new Tree();
            AddBasicDataTypes();
        }

        private void AddBasicDataTypes()
        {
            lDataType.Add(new DataType());
            lDataType.Last().TypeName = "INTEGER";
            lDataType.Last().oRange.Min = -2147483648;
            lDataType.Last().oRange.Max = 2147483647;
            lDataType.Last().oOtherData.Visibility = "UNIVERSAL";
            lDataType.Last().oOtherData.TypeID = 2;
            lDataType.Add(new DataType());
            lDataType.Last().TypeName = "OCTET STRING";
            lDataType.Last().oOtherData.Visibility = "UNIVERSAL";
            lDataType.Last().oOtherData.TypeID = 4;
            lDataType.Add(new DataType());
            lDataType.Last().TypeName = "NULL";
            lDataType.Last().oOtherData.Visibility = "UNIVERSAL";
            lDataType.Last().oOtherData.TypeID = 5;
            lDataType.Add(new DataType());
            lDataType.Last().TypeName = "OBJECT IDENTIFIER";
            lDataType.Last().oOtherData.Visibility = "UNIVERSAL";
            lDataType.Last().oOtherData.TypeID = 6;
            lDataType.Add(new DataType());
            lDataType.Last().TypeName = "SEQUENCE";
            lDataType.Last().oOtherData.Visibility = "UNIVERSAL";
            lDataType.Last().oOtherData.TypeID = 16;


        }

        public void GenerateTree()
        {
            oTree.FindRoot();
        }

        public ObjectType FindByOID()
        {
            Console.Write("Enter OID of Object: ");
            sOID = Console.ReadLine();
            if (sOID == "q")
                return null;
            Regex RegexOID = new Regex(@"(?<OID>\d+)", RegexOptions.Singleline | RegexOptions.Compiled);
            MatchCollection Matches = RegexOID.Matches(sOID);
            ObjectType _TempObjectType = new ObjectType();

            if (Int32.Parse(Matches[0].Groups[1].Value) == oTree.oRoot.oObjectType.OID)
            {
                if (Matches[0].NextMatch().Success)
                    _TempObjectType = FindNextOID(oTree.oRoot.Childrens, Matches[0].NextMatch());
                else
                    _TempObjectType = oTree.oRoot.oObjectType;
                if (_TempObjectType != null)
                {
                    return _TempObjectType;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nObject with OID {0} doesn't exist!. Try again...\n", sOID);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    return FindByOID();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nObject with OID {0} doesn't exist!. Try again...\n", sOID);
                Console.ForegroundColor = ConsoleColor.Gray;
                return FindByOID();
            }
        }

        private ObjectType FindNextOID(List<TreeLeaf> _LeafList, Match _match)
        {
            foreach (TreeLeaf _Leaf in _LeafList)
            {
                if (_Leaf.oObjectType.OID == Int32.Parse(_match.Groups[1].Value))
                {
                    if (_match.NextMatch().Success)
                        return FindNextOID(_Leaf.Childrens, _match.NextMatch());
                    else
                        return _Leaf.oObjectType;
                }
            }
            return null;
        }

        public void PrintObjectTree()
        {
            oTree.oRoot.oObjectType.PresentData(iWriteIteration);
            PrintObjectChildrens(oTree.oRoot.Childrens);
        }

        private void PrintObjectChildrens(List<TreeLeaf> _LeafList)
        {
            iWriteIteration++;
            foreach (TreeLeaf _Leaf in _LeafList)
            {
                _Leaf.oObjectType.PresentData(iWriteIteration);
                PrintObjectChildrens(_Leaf.Childrens);
                iWriteIteration--;
            }
        }

        public void PrintDataTypes()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n\n--------------------------------------------------------------");
            Console.WriteLine("                 Znaleziono {0} typów danych.                 ", lDataType.Count());
            foreach(DataType _DataType in lDataType)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("--------------------------------{0}-----------------------------", lDataType.IndexOf(_DataType));
                _DataType.PresentData(0);
            }
        }

        public DataType FindDataTypeByName(string _sName)
        {
            foreach (DataType _DataType in lDataType)
            {
                if (_DataType.TypeName == _sName)
                {
                    return _DataType;
                }
            }
            return null;
        }
    }
}
