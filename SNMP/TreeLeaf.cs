using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex_example
{
    public class TreeLeaf
    {
        public List<TreeLeaf> Childrens;
        public Data oData;
        public TreeLeaf()
        {
            oData = new Data();
            Childrens = new List<TreeLeaf>();
        }
    }
}
