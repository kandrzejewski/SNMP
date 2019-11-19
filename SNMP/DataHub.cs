using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                _DataType.PresentData();
            }
        }
    }
}
