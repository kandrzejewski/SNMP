using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex_example
{
    class DataHub
    {
        public static List<Data> lData;
        public Tree oTree;
        private int iWriteIteration = 0;
        public DataHub()
        {
            lData = new List<Data>();
            oTree = new Tree();
        }

        public void GenerateTree()
        {
            oTree.FindRoot();
            //WriteTree();
            
        }
        public void WriteTree()
        {

            oTree.oRoot.oData.WriteData(0);
            WriteChildren(oTree.oRoot.Childrens);
        }
        private void WriteChildren(List<TreeLeaf> _LeafList)
        {
            iWriteIteration++;
            foreach (TreeLeaf _Leaf in _LeafList)
            {
                _Leaf.oData.WriteData(iWriteIteration);
                WriteChildren(_Leaf.Childrens);
                iWriteIteration = 1;

            }
        }
    }

}
