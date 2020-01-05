using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    class Encoder
    {
        DataHub oDataHub = new DataHub();
        DataType oDataType = new DataType();

        public Encoder(DataHub _oDataHub)
        {
            oDataHub = _oDataHub;
        }

        public void EncodingInit(EncoderData _oEncoderData)
        {
            Console.Clear();
            Console.WriteLine("Jestem w Koderze!\n---------------------------------------------------------------------\n\n");
            if (_oEncoderData.bCanEncode)
            {
                _oEncoderData.PresentData();
                Console.WriteLine("\nEncoding...");
                DataTypeClassification(_oEncoderData._oDataType, _oEncoderData.ValueToEncode);
            }
            else
            {
                _oEncoderData.PresentData();
                Console.WriteLine("\nReturning to menu.");
            }
        }

        private void DataTypeClassification(DataType _oDataType, List<string> _ValueToEncode)
        {
            if (_oDataType.oSequence.lElements.Count == 0)
            {
                if (_oDataType.oOtherData.ParrentType == null)
                {
                    Encode(_ValueToEncode[0], _oDataType.oOtherData.Class, _oDataType.oOtherData.TagNumber);
                }
                else
                {
                    if (_oDataType.oOtherData.EncodingType == "EXPLICIT")
                    {
                        oDataType = oDataHub.FindDataTypeByName(_oDataType.oOtherData.ParrentType);
                        Console.WriteLine(oDataType.TypeName);
                        while (oDataType.oOtherData.ParrentType != null)
                        {
                            oDataType = oDataHub.FindDataTypeByName(oDataType.oOtherData.ParrentType);
                            Console.WriteLine(oDataType.TypeName);
                        }
                        Encode(_ValueToEncode[0], _oDataType.oOtherData.Class, oDataType.oOtherData.Class, _oDataType.oOtherData.TagNumber, oDataType.oOtherData.TagNumber);
                    }
                    else
                    {
                        oDataType = oDataHub.FindDataTypeByName(_oDataType.oOtherData.ParrentType);
                        Console.WriteLine(oDataType.TypeName);
                        while (oDataType.oOtherData.ParrentType != null)
                        {
                            oDataType = oDataHub.FindDataTypeByName(oDataType.oOtherData.ParrentType);
                            Console.WriteLine(oDataType.TypeName);
                        }
                        Encode(_ValueToEncode[0], _oDataType.oOtherData.Class, _oDataType.oOtherData.TagNumber, oDataType.TypeName);
                    }

                }
            }
            else
            {

            }
        }

        private void Encode(string _ValueToEncode, string _Class, int _TagNumber)
        {
            switch (_TagNumber)
            {
                case 2:
                    {
                        Console.WriteLine("Koduję se Integera!");
                        break;
                    }
                case 4:
                    {
                        Console.WriteLine("Koduję se Octeta!");
                        break;
                    }
                case 5:
                    {
                        Console.WriteLine("Koduję se Nulla!");
                        break;
                    }
                case 6:
                    {
                        Console.WriteLine("Koduję se Objecta!");
                        break;
                    }
            }
        }

        private void Encode(string _ValueToEncode, string _Class, int _TagNumber, string _ParrentName)
        {
            switch (_ParrentName)
            {
                case "INTEGER":
                    {
                        Console.WriteLine("Koduję se ten typ jako Int!");
                        break;
                    }
                case "OCTET STRING":
                    {
                        Console.WriteLine("Koduję se ten typ jako Octeta!");
                        break;
                    }
                case "OBJECT IDENTIFIER":
                    {
                        Console.WriteLine("Koduję se ten typ jako Objecta!");
                        break;
                    }
            }
        }
        private void Encode(string _ValueToEncode, string _Class, string _ParrentClass, int _TagNumber, int _ParrentTagNumber)
        {
            switch (_ParrentTagNumber)
            {
                case 2:
                    {
                        Console.WriteLine("Koduję se EXPLICIT ten typ jako integer");
                        break;
                    }
                case 4:
                    {
                        Console.WriteLine("Koduję se EXPLICIT ten typ jako Octeta!");
                        break;
                    }
                case 5:
                    {
                        Console.WriteLine("Koduję se EXPLICIT ten typ jako Nulla!");
                        break;
                    }
                case 6:
                    {
                        Console.WriteLine("Koduję se EXPLICIT ten typ jako Objecta!");
                        break;
                    }
            }
        }
    }
}
