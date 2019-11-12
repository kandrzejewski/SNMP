using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex_example
{
    public class Tree
    {
        public TreeLeaf oRoot;
        public Tree()
        {
            oRoot = new TreeLeaf();
        }
        public void FindRoot()
        {
            foreach(Data _Data in DataHub.lData)
            {
                if (_Data.parrentId == 0)
                {
                    oRoot.oData = _Data;
                }
            }
            FindChildrens(oRoot);
        }
        public void FindChildrens(TreeLeaf _Parrent)
        {
            foreach (Data _Data in DataHub.lData)
            {
                if (_Data.parrentId == _Parrent.oData.id)
                {
                    _Parrent.Childrens.Add(new TreeLeaf());
                    _Parrent.Childrens.Last().oData = _Data;
                    FindChildrens(_Parrent.Childrens.Last());
                }
            }
        }
    }
}
