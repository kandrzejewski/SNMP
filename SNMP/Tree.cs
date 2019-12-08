using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    class Tree
    {
        public TreeLeaf oRoot;
        public Tree()
        {
            oRoot = new TreeLeaf();
        }
        public void FindRoot()
        {
            foreach(ObjectType _Data in DataHub.lData)
            {
                if (_Data.Name == "iso")
                {
                    oRoot.oObjectType = _Data;
                }
            }
            FindChildrens(oRoot);
        }
        private void FindChildrens(TreeLeaf _Parrent)
        {
            foreach (ObjectType _Data in DataHub.lData)
            {
                if (_Data.ParrentName == _Parrent.oObjectType.Name)
                {
                    _Parrent.Childrens.Add(new TreeLeaf());
                    _Parrent.Childrens.Last().oObjectType = _Data;
                    FindChildrens(_Parrent.Childrens.Last());
                }
            }
        }
    }
}
