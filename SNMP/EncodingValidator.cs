using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace SNMP
{
    class EncodingValidator
    {
        private EncoderData oResult = new EncoderData();
        private byte iteration = 0;
        private bool bSendDataType = false;

        public EncoderData ValidateAnyDataType(DataHub oDataHub)
        {
            string sNameOfType;
            oResult = new EncoderData();
            DataType oDataType = new DataType();

            Console.Write("Enter a name of Type: ");
            sNameOfType = Console.ReadLine();
            if(sNameOfType != "SEQUENCE")
            {
                oDataType = oDataHub.FindDataTypeByName(sNameOfType);
                if (oDataType != null)
                {
                    oResult = Validate("\n\nFound Data Type " +
                    oDataType.TypeName, oDataType);
                }
                else
                {
                    oResult.sErrorDescription = "Data Type " + sNameOfType + " not found!";
                    oResult.bCanEncode = false;
                }
            }
            else
            {
                Console.WriteLine("\n\nCreate a sequence to encode.");
                oDataType = CreateSequence(oDataHub);
                oResult = Validate("\n\nCreated Sequence: ", oDataType);
            }

            return oResult;
        }

        private DataType CreateSequence(DataHub oDataHub)
        {
            int iSequenceIndex = 0;
            DataType _oDataType = new DataType();
            bool SequenceComplete = false;
            string SequenceChoice = string.Empty;
            string sDataTypeName;

            while (!SequenceComplete)
            {
                SequenceChoice = string.Empty;
                Console.Write("\nEnter a {0} Data Type name: ", iSequenceIndex++);
                _oDataType.oSequence.lElements.Add(new SequenceElement());
                _oDataType.oSequence.lElements.Last().ElementName = iSequenceIndex.ToString();
                sDataTypeName = Console.ReadLine();
                if (sDataTypeName == "SEQUENCE")
                {
                    Console.WriteLine("\n\n────────────────────────────────────────────────────\n" + 
                        "Create a new sequence at {0} position of parent sequence." +
                        "\n─────────────────────────────────────────────────────────────", iSequenceIndex);
                    _oDataType.oSequence.lElements.Last().ElementType = CreateSequence(oDataHub);
                }
                else
                {
                    _oDataType.oSequence.lElements.Last().ElementType = oDataHub.FindDataTypeByName(sDataTypeName);
                }
                while (SequenceChoice != "Y" && SequenceChoice != "y" && SequenceChoice != "N" && SequenceChoice != "n")
                {
                    Console.WriteLine("\n\nNext data type in sequece? (Y/N)");
                    Console.Write("\n=>");
                    SequenceChoice = Console.ReadLine();
                }
                if (SequenceChoice == "N" || SequenceChoice == "n")
                {
                    SequenceComplete = true;
                    Console.WriteLine("\n────────────────────────────────────────────────────" +
                        "\nEnd of Sequence.\n────────────────────────────────────────────────────");
                }
            }
            return _oDataType;
        }

        public EncoderData Validate(string _title, DataType _oDataType)
        {
            //Console.WriteLine("Iteration: " + iteration);
            //Console.WriteLine("Count: " + oResult.ValueToEncode.Count);
            if (_oDataType != null)
            {
                if (iteration == 0)
                {
                    if (!bSendDataType)
                    {
                        oResult.ValueToEncode.Add(string.Empty);
                        oResult._oDataType = _oDataType;
                        bSendDataType = true;
                    }
                }
                if (_oDataType.oSequence.lElements.Count > 1 || _oDataType.oSequence.lElements.Count == 0)
                {
                    Console.WriteLine(_title);
                    Console.WriteLine("────────────────────────────────────────────────────");
                    _oDataType.PresentData(1);
                    Console.WriteLine("────────────────────────────────────────────────────");
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                if (_oDataType.TypeName == "INTEGER" || _oDataType.oOtherData.ParrentType == "INTEGER")
                {
                    oResult.bCanEncode = IntegerValidation(_oDataType);
                    return oResult;
                }
                else if (_oDataType.TypeName == "OCTET STRING" || _oDataType.oOtherData.ParrentType == "OCTET STRING")
                {
                    oResult.bCanEncode = OctetStringValidation(_oDataType);
                    return oResult;
                }
                else if (_oDataType.TypeName == "OBJECT IDENTIFIER" || _oDataType.oOtherData.ParrentType == "OBJECT IDENTIFIER")
                {
                    oResult.bCanEncode = ObjectIdentifierValidation(_oDataType);
                    return oResult;
                }
                else if (_oDataType.TypeName == "NULL")
                {
                    oResult.bCanEncode = true;
                    return oResult;
                }
                else if (_oDataType.oSequence.lElements.Count() == 0)
                {
                    if (!_oDataType.EmptyCheck(_oDataType.oOtherData.ParrentType))
                    {
                        foreach (DataType _DataType in DataHub.lDataType)
                        {
                            if (_oDataType.oOtherData.ParrentType == _DataType.TypeName)
                                if (Validate("Parrent of Data Type of Object " + _oDataType.TypeName, _DataType).bCanEncode)
                                {
                                    if (_DataType.TypeName == "INTEGER" || _DataType.oOtherData.ParrentType == "INTEGER")
                                    {
                                        oResult.bCanEncode = IntegerValidation(_oDataType, Int64.Parse(oResult.ValueToEncode[iteration]));
                                        return oResult;
                                    }
                                    if (_DataType.TypeName == "OCTET STRING" || _DataType.oOtherData.ParrentType == "OCTET STRING")
                                    {
                                        oResult.bCanEncode = OctetStringValidation(_oDataType, oResult.ValueToEncode[iteration]);
                                        return oResult;
                                    }
                                    if (_DataType.TypeName == "OBJECT IDENTIFIER" || _DataType.oOtherData.ParrentType == "OBJECT IDENTIFIER") {
                                        oResult.bCanEncode = ObjectIdentifierValidation(_oDataType, oResult.ValueToEncode[iteration]);
                                        return oResult;
                                    }
                                }
                        }
                    }
                }
                else if (_oDataType.oSequence.lElements.Count > 0)
                {
                    foreach (SequenceElement _SequenceElement in _oDataType.oSequence.lElements)
                    {
                        if (_SequenceElement != _oDataType.oSequence.lElements[0])
                        {
                            oResult.ValueToEncode.Add(string.Empty);
                        }
                        if (!Validate("\n\n" + (_oDataType.oSequence.lElements.IndexOf(_SequenceElement) + 1) 
                            + " element of Sequence:", _SequenceElement.ElementType).bCanEncode)
                        {
                            oResult.bCanEncode = false;
                            return oResult;
                        }
                        if(!(_SequenceElement == _oDataType.oSequence.lElements.Last()))
                            iteration++;
                    }
                    oResult.bCanEncode = true;
                    return oResult;
                }
                oResult.sErrorDescription = "Parameters of Data Type is not recognized!";
                oResult.bCanEncode = false;
                return oResult;
            }
            else
            {
                oResult.sErrorDescription = "Data Type of chosen Object not found!";
                oResult.bCanEncode = false;
                return oResult;
            }

        }      

        private bool IntegerValidation(DataType _oDataType)
        {
            long iValueToEncode;
            bool bConversionStatus;
            if(_oDataType.TypeName !="")
                Console.WriteLine("Enter a Value:\n({0}\nRange {1}..{2})\n",_oDataType.TypeName, 
                    _oDataType.oRange.Min, _oDataType.oRange.Max);
            else
                Console.WriteLine("Enter a Value:\n({0}\nRange {1}..{2})\n", _oDataType.oOtherData.ParrentType,
                    _oDataType.oRange.Min, _oDataType.oRange.Max);
            Console.Write("=>");
            oResult.ValueToEncode[iteration] = Console.ReadLine();
            bConversionStatus = Int64.TryParse(oResult.ValueToEncode[iteration], out iValueToEncode);

            if (bConversionStatus)
            {
                if (iValueToEncode < _oDataType.oRange.Min)
                {
                    oResult.sErrorDescription = "Entered value is out of range! Min value is " + _oDataType.oRange.Min;
                    return false;
                }

                if (!_oDataType.EmptyCheck(_oDataType.oRange.Max))
                    if (iValueToEncode > _oDataType.oRange.Max)
                    {
                        oResult.sErrorDescription = "Entered value is out of range! Max value is " + _oDataType.oRange.Max;
                        return false;
                    }
            }
            else
            {
                oResult.sErrorDescription = "Value is incorrect!";
                return false;
            }
            return true;
        }

        private bool IntegerValidation(DataType _oDataType, long _Value)
        {
            if (_Value < _oDataType.oRange.Min)
            {
                oResult.sErrorDescription = "Entered value is out of range! Min value is " + _oDataType.oRange.Min;
                return false;
            }

            if (!_oDataType.EmptyCheck(_oDataType.oRange.Max))
                if (_Value > _oDataType.oRange.Max)
                {
                    oResult.sErrorDescription = "Entered value is out of range! Max value is " + _oDataType.oRange.Max;
                    return false;
                }
            return true;
        }

        private bool OctetStringValidation(DataType _oDataType)
        {   
            if(_oDataType.oSize.Size !=0)
                Console.WriteLine("Enter a Value\n({0}\nRange 0..{1} bytes)\n", _oDataType.TypeName,
                     _oDataType.oSize.Size);
            else
                Console.WriteLine("Enter a Value:\n({0})\n", _oDataType.TypeName);

            Console.Write("=>");
            oResult.ValueToEncode[iteration] = Console.ReadLine();
            if (!_oDataType.EmptyCheck(_oDataType.oSize.Size))
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(oResult.ValueToEncode[iteration]) > _oDataType.oSize.Size)
                {
                    oResult.sErrorDescription = "Entered value is out of range! Max size of value is " + _oDataType.oSize.Size + " bytes!";
                    return false;
                }
            if (!_oDataType.EmptyCheck(_oDataType.oRange.Max))
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(oResult.ValueToEncode[iteration]) > _oDataType.oRange.Max / 8)
                {
                    oResult.sErrorDescription = "Entered value is out of range! Max size of value is " + ((_oDataType.oRange.Max / 8) + 1) + " bytes!";
                    return false;
                }
            return true;
        }

        private bool OctetStringValidation(DataType _oDataType, string _value)
        {     
            if (!_oDataType.EmptyCheck(_oDataType.oSize.Size))
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(_value) > _oDataType.oSize.Size)
                {
                    oResult.sErrorDescription = "Entered value is out of range! Max size of value is " + _oDataType.oSize.Size + " bytes!";
                    return false;
                }
            if (!_oDataType.EmptyCheck(_oDataType.oRange.Max))
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(_value) > _oDataType.oRange.Max/8)
                {
                    oResult.sErrorDescription = "Entered value is out of range! Max size of value is " + ((_oDataType.oRange.Max / 8) + 1) + " bytes!";
                    return false;
                }
                return true;
        }

        private bool ObjectIdentifierValidation(DataType _oDataType)
        {
            Console.WriteLine("Enter a Value\n(Range 0..N\nFormat: OID1 OID2 ... OIDn)");
            Console.Write("=>");
            oResult.ValueToEncode[iteration] = Console.ReadLine();
            Regex RegexOID = new Regex(@"\S+", RegexOptions.Singleline | RegexOptions.Compiled);
            MatchCollection OIDS = RegexOID.Matches(oResult.ValueToEncode[iteration]);
            int _iOID;
            foreach (Match _match in OIDS)
            {
                if (Int32.TryParse(_match.Value, out _iOID))
                {
                    if (_iOID < 0)
                    {
                        oResult.sErrorDescription = "Entered value is less than 0. OID must be a positive value!";
                        return false;
                    }
                }
                else
                {
                    oResult.sErrorDescription = "Entered value is incorrect. OID must be a positive integer value!";
                    return false;
                }
            }  
            return true;
        }

        private bool ObjectIdentifierValidation(DataType _oDataType, string _value)
        {
            MatchCollection OIDS = Regex.Matches(@"\S+", _value, RegexOptions.Singleline | RegexOptions.Compiled);

            foreach (Match _match in OIDS)
            {
                int _OID;
                if (Int32.TryParse(_match.Groups[0].Value, out _OID))
                {
                    if (_OID < 0)
                    {
                        oResult.sErrorDescription = "Entered value is less than 0. OID must be a positive value!";
                        return false;
                    }
                }
                else
                {
                    oResult.sErrorDescription = "Entered value is incorrect. OID must be a positive integer value!";
                    return false;
                }
            }
            return true;
        }
    }
}