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
        private int iWriteIteration = 0;

        public DataHub()
        {
            lData = new List<ObjectType>();
            lDataType = new List<DataType>();
            oTree = new Tree();
        }

        public void GenerateTree()
        {
            oTree.FindRoot();
        }

        public ObjectType FindByOID()
        {
            string sOID;
            Console.Write("Podaj OID poszukiwanego obiektu: ");
            sOID = Console.ReadLine();

            Regex RegexOID = new Regex(@"(?<OID>\d+)", RegexOptions.Singleline | RegexOptions.Compiled);
            MatchCollection Matches = RegexOID.Matches(sOID);

            if (Int32.Parse(Matches[0].Groups[1].Value) == oTree.oRoot.oData.OID)
            {
                ObjectType _TempObjectType = FindNext(oTree.oRoot.Childrens, Matches[0].NextMatch());
                if (_TempObjectType != null)
                {
                    return _TempObjectType;
                }           
                else
                {
                    Console.WriteLine("Objekt o padoanym OID nie istnieje!. Spróbuj ponownie.\n");
                    return FindByOID();
                }
            }
            else
            {
                Console.WriteLine("Objekt o padoanym OID nie istnieje!. Spróbuj ponownie.\n");
                return FindByOID();
            }
        }

        private ObjectType FindNext(List<TreeLeaf> _LeafList, Match _match)
        {
            foreach (TreeLeaf _Leaf in _LeafList)
            {
                if (_Leaf.oData.OID == Int32.Parse(_match.Groups[1].Value))
                {
                    if (_match.NextMatch().Success)
                        return FindNext(_Leaf.Childrens, _match.NextMatch());
                    else
                        return _Leaf.oData;
                }
            }
            return null;
        }

        public void WriteTree()
        {
            oTree.oRoot.oData.PresentData(iWriteIteration);
            WriteChildren(oTree.oRoot.Childrens);
        }

        private void WriteChildren(List<TreeLeaf> _LeafList)
        {
            iWriteIteration++;
            foreach (TreeLeaf _Leaf in _LeafList)
            {
                _Leaf.oData.PresentData(iWriteIteration);
                WriteChildren(_Leaf.Childrens);
                iWriteIteration--;
            }
        }
        public void WriteTypes()
        {
            foreach(DataType _DataType in lDataType)
            {
                _DataType.PresentData(0);
            }
        }
    }
}
