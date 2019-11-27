using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    public class Program
    {

        static void Main(string[] args)
        {
            Pharser oPharser = new Pharser();
            DataHub oDataHub = new DataHub();

            oPharser.RunPharser("RFC1213-MIB");
            oDataHub.GenerateTree();
            oDataHub.WriteTree();
            //oDataHub.WriteTypes();

            ObjectType oOT = new ObjectType();
            oOT = oDataHub.FindByOID();

            oOT.PresentData(0);
            
        }
    }
}
