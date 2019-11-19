using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    class Pharser: DataHub
    {
        public void RunPharser(string filename)
        {
            Pharse oPharse = new Pharse();
            oPharse.ReadFromFile(filename);           
        }
    }
}
