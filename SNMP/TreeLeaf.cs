using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    public class TreeLeaf
    {
        public List<TreeLeaf> Childrens;
        public ObjectType oData;
        public TreeLeaf()
        {
            oData = new ObjectType();
            Childrens = new List<TreeLeaf>();
        }
    }
}
