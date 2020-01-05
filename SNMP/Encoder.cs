using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    class Encoder
    {
        DataType oDataType = new DataType();

        public Encoder()
        {
        }

        public void EncodingInit(EncoderData _oEncoderData, DataHub _oDataHub)
        {
            Console.Clear();
            Console.WriteLine("Jestem w Koderze!\n---------------------------------------------------------------------\n\n");
            if (_oEncoderData.bCanEncode)
            {
                _oEncoderData.PresentData();
                Console.WriteLine("\nEncoding...");
                DataTypeClassification(_oEncoderData._oDataType, _oEncoderData.ValueToEncode, _oDataHub);
            }
            else
            {
                _oEncoderData.PresentData();
                Console.WriteLine("\nReturning to menu.");
            }
        }

        private void DataTypeClassification(DataType _oDataType, List<string> _ValueToEncode, DataHub _oDataHub)
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
                        oDataType = _oDataHub.FindDataTypeByName(_oDataType.oOtherData.ParrentType);
                        Console.WriteLine(oDataType.TypeName);
                        while (oDataType.oOtherData.ParrentType != null)
                        {
                            oDataType = _oDataHub.FindDataTypeByName(oDataType.oOtherData.ParrentType);
                            Console.WriteLine(oDataType.TypeName);
                        }
                        Encode(_ValueToEncode[0], _oDataType.oOtherData.Class, oDataType.oOtherData.Class, _oDataType.oOtherData.TagNumber, oDataType.oOtherData.TagNumber);
                    }
                    else
                    {
                        oDataType = _oDataHub.FindDataTypeByName(_oDataType.oOtherData.ParrentType);
                        Console.WriteLine(oDataType.TypeName);
                        while (oDataType.oOtherData.ParrentType != null)
                        {
                            oDataType = _oDataHub.FindDataTypeByName(oDataType.oOtherData.ParrentType);
                            Console.WriteLine(oDataType.TypeName);
                        }
                        Encode(_ValueToEncode[0], _oDataType.oOtherData.Class, _oDataType.oOtherData.TagNumber, oDataType.TypeName);
                    }

                }
            }
            else
            {
                int Iterator = 0;
                List<String> Value = new List<string>();
                Value.Add(null);
                if (_oDataType.oOtherData.ParrentType == null)
                {
                    foreach (SequenceElement oSequenceElement in _oDataType.oSequence.lElements)
                    {
                        Value[0] = _ValueToEncode[Iterator];
                        DataTypeClassification(oSequenceElement.ElementType, Value, _oDataHub);
                    }
                    //EncodeSequence(_oDataType.oOtherData.);
                }
                else
                {

                }
            }
        }

        private void Encode(string _ValueToEncode, string _Class, int _TagNumber)
        {
            switch (_TagNumber)
            {
                case 2:
                    {
                        Console.WriteLine("Koduję se Integera!");
                        EncodeIdentifier(_Class, false, _TagNumber);
                        break;
                    }
                case 4:
                    {
                        Console.WriteLine("Koduję se Octeta!");
                        EncodeIdentifier(_Class, false, _TagNumber);
                        break;
                    }
                case 5:
                    {
                        Console.WriteLine("Koduję se Nulla!");
                        EncodeIdentifier(_Class, false, _TagNumber);
                        break;
                    }
                case 6:
                    {
                        Console.WriteLine("Koduję se Objecta!");
                        EncodeIdentifier(_Class, false, _TagNumber);
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
                        EncodeIdentifier(_Class, false, _TagNumber);
                        break;
                    }
                case "OCTET STRING":
                    {
                        Console.WriteLine("Koduję se ten typ jako Octeta!");
                        EncodeIdentifier(_Class, false, _TagNumber);
                        break;
                    }
                case "OBJECT IDENTIFIER":
                    {
                        Console.WriteLine("Koduję se ten typ jako Objecta!");
                        EncodeIdentifier(_Class, false, _TagNumber);
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

        private void EncodeIdentifier(string _Class, bool _Complexity, int _TagNumber)
        {
            byte EncodedValue;
            byte Class;
            byte Complexity;
            byte TagNumber;

            switch (_Class)
            {
                case "UNIVERSAL":
                    Class = 0;
                    break;
                case "APPLICATION":
                    Class = 64;
                    break;
                case "CONTEXT-SPECIFIC":
                    Class = 128;
                    break;
                case "PRIVATE":
                    Class = 192;
                    break;
                default:
                    Class = 0;
                    break;
            }
            if (_Complexity == false)
                Complexity = 0;
            else
                Complexity = 32;
            if (_TagNumber < 31)
            {
                TagNumber = Convert.ToByte(_TagNumber);
            }
            else
                TagNumber = 0;

            EncodedValue = Convert.ToByte(Class + Complexity + TagNumber);
            Console.WriteLine("Zakodowany Identyfikator: " + EncodedValue);
            Console.WriteLine("Zakodowany Identyfikator: " + Int32.Parse(Convert.ToString(EncodedValue, 2)).ToString("0000 0000"));
        }
    }
}
