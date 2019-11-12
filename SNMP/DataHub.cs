using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex_example
{
    public class DataHub
    {
        public static List<Data> lData;
        Tree oTree;
        public DataHub()
        {
            lData = new List<Data>();
            oTree = new Tree();
        }

        public void GenerateTree()
        {
            oTree.FindRoot();
            WriteTree();
            
        }
        public void WriteTree()
        {
            oTree.oRoot.oData.WriteData();
            WriteChildren(oTree.oRoot.Childrens);
        }
        public void WriteChildren(List<TreeLeaf> _LeafList)
        {
            foreach (TreeLeaf _Leaf in _LeafList)
            {
                _Leaf.oData.WriteData();
                WriteChildren(_Leaf.Childrens);
            }
        }
    }

}
