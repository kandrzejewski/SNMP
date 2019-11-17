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
                if (_Data.OID == 0)
                {
                    oRoot.oData = _Data;
                }
            }
            FindChildrens(oRoot);
        }
        private void FindChildrens(TreeLeaf _Parrent)
        {
            foreach (ObjectType _Data in DataHub.lData)
            {
                if (_Data.ParrentName == _Parrent.oData.Name)
                {
                    _Parrent.Childrens.Add(new TreeLeaf());
                    _Parrent.Childrens.Last().oData = _Data;
                    FindChildrens(_Parrent.Childrens.Last());
                }
            }
        }
    }
}
