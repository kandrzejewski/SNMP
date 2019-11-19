using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    public class Program
    {
        public static List<ObjectType> DataList;

        static void Main(string[] args)
        {
            Pharser oPharser = new Pharser();
            DataHub oDataHub = new DataHub();

            oPharser.RunPharser("RFC1213-MIB");
            oDataHub.GenerateTree();
            oDataHub.WriteTree();
            
            //oDataHub.WriteTypes();
        }
    }
}
