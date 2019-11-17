using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace SNMP
{
    class Pharser: DataHub
    {
        public void RunPharser()
        {
            Pharse oPharse = new Pharse();
            oPharse.ReadFromFile("RFC1213-MIB");           
        }
    }
}
