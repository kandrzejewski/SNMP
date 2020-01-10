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
        
        public EncoderData ValidateAnyDataType(DataHub _oDataHub)
        {
            string sNameOfType;
            oResult = new EncoderData();
            DataType oDataType = new DataType();

            Console.Write("Enter a name of Type: ");
            sNameOfType = Console.ReadLine().ToUpper();
            if(sNameOfType != "SEQUENCE")
            {
                oDataType = _oDataHub.FindDataTypeByName(sNameOfType);
                if (oDataType != null)
                {
                    oResult = Validate("\n\nFound Data Type " +
                    oDataType.TypeName, oDataType, _oDataHub);
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
                oDataType = CreateSequence(_oDataHub);
                oResult = Validate("\n\nCreated Sequence: ", oDataType, _oDataHub);
            }

            return oResult;
        }

        private DataType CreateSequence(DataHub oDataHub)
        {
            int iSequenceIndex = 0;
            DataType _oDataType = CloneDataType(oDataHub.FindDataTypeByName("SEQUENCE"));
            bool SequenceComplete = false;
            string SequenceChoice = string.Empty;
            string sDataTypeName;

            while (!SequenceComplete)
            {
                SequenceChoice = string.Empty;
                Console.Write("\nEnter a {0} Data Type name: ", iSequenceIndex++);
                _oDataType.oSequence.lElements.Add(new SequenceElement());
                _oDataType.oSequence.lElements.Last().ElementName = iSequenceIndex.ToString();
                sDataTypeName = Console.ReadLine().ToUpper();
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

        public EncoderData Validate(string _title, DataType _oDataType, DataHub _oDataHub)
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
                if (_oDataType.TypeName == "INTEGER")
                {
                    oResult.bCanEncode = IntegerValidation(_oDataType);
                    return oResult;
                }
                else if (_oDataType.TypeName == "OCTET STRING")
                {
                    oResult.bCanEncode = OctetStringValidation(_oDataType);
                    return oResult;
                }
                else if (_oDataType.TypeName == "OBJECT IDENTIFIER")
                {
                    oResult.bCanEncode = ObjectIdentifierValidation(_oDataType);
                    return oResult;
                }
                else if (_oDataType.TypeName == "NULL")
                {
                    oResult.bCanEncode = true;
                    return oResult;
                }
                else if (_oDataType.TypeName == "BOOLEAN")
                {
                    oResult.bCanEncode = BooleanValidation(_oDataType);
                    return oResult;
                }
                else if (_oDataType.oSequence.lElements.Count() == 0)
                {
                    if (!_oDataType.EmptyCheck(_oDataType.oOtherData.ParrentType))
                    {
                        DataType oNewDataType = new DataType();
                        oNewDataType = CloneDataType(_oDataHub.FindDataTypeByName(_oDataType.oOtherData.ParrentType));
                        if (oNewDataType != null)
                        {
                            if (_oDataType.oRange.Max != 0)
                            {
                                if (oNewDataType.oRange.Max > _oDataType.oRange.Max || oNewDataType.oRange.Max == 0)
                                {
                                    oNewDataType.oRange.Max = _oDataType.oRange.Max;
                                }
                                if (oNewDataType.oRange.Min < _oDataType.oRange.Min)
                                {
                                    oNewDataType.oRange.Min = _oDataType.oRange.Min;
                                }
                            }
                            if (_oDataType.oSize.Size != 0)
                                if (oNewDataType.oSize.Size > _oDataType.oSize.Size || oNewDataType.oSize.Size == 0)
                                    oNewDataType.oSize.Size = _oDataType.oSize.Size;
                        }

                        if (Validate("Parrent of Data Type " + _oDataType.TypeName + " with child restrictions", oNewDataType, _oDataHub).bCanEncode)
                        {
                            if (oNewDataType.TypeName == "INTEGER")
                            {
                                oResult.bCanEncode = IntegerValidation(_oDataType, Int64.Parse(oResult.ValueToEncode[iteration]));
                                return oResult;
                            }
                            if (oNewDataType.TypeName == "OCTET STRING")
                            {
                                oResult.bCanEncode = OctetStringValidation(_oDataType, oResult.ValueToEncode[iteration]);
                                return oResult;
                            }
                            if (oNewDataType.TypeName == "OBJECT IDENTIFIER")
                            {
                                oResult.bCanEncode = ObjectIdentifierValidation(_oDataType, oResult.ValueToEncode[iteration]);
                                return oResult;
                            }
                            if (oNewDataType.TypeName == "BOOLEAN")
                            {
                                oResult.bCanEncode = BooleanValidation(_oDataType, oResult.ValueToEncode[iteration]);
                                return oResult;
                            }
                        }
                        return oResult;
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
                            + " element of Sequence:", _SequenceElement.ElementType, _oDataHub).bCanEncode)
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
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(oResult.ValueToEncode[iteration]) > (_oDataType.oRange.Max + 1) / 256)
                {
                    oResult.sErrorDescription = "Entered value is out of range! Max size of value is " + ((_oDataType.oRange.Max / 256) + 1) + " bytes!";
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

        private bool BooleanValidation(DataType _oDataType)
        {
            Console.WriteLine("Enter a Value\n(true or false)");
            Console.Write("=>");
            oResult.ValueToEncode[iteration] = Console.ReadLine().ToLower();
            if (oResult.ValueToEncode[iteration] != "true" && oResult.ValueToEncode[iteration] != "false")
            {
                    oResult.sErrorDescription = "Entered value is incorrect. BOOLEAN can take only true and false values!";
                    return false;
            }
            else
            {
                return true;
            }
        }

        private bool BooleanValidation(DataType _oDataType, string _value)
        {
            if (oResult.ValueToEncode[iteration] != "true" && oResult.ValueToEncode[iteration] != "false")
            {
                oResult.sErrorDescription = "Entered value is incorrect. BOOLEAN can take only true and false values!";
                return false;
            }
            else
            {
                return true;
            }
        }

        private DataType CloneDataType(DataType _CloningDataType)
        {
            DataType NewDataType = new DataType();
            if (!_CloningDataType.EmptyCheck(_CloningDataType.TypeName))
                NewDataType.TypeName = _CloningDataType.TypeName;
            if (!_CloningDataType.EmptyCheck(_CloningDataType.oOtherData.Class))
                NewDataType.oOtherData.Class = _CloningDataType.oOtherData.Class;
            if (!_CloningDataType.EmptyCheck(_CloningDataType.oOtherData.EncodingType))
                NewDataType.oOtherData.EncodingType = _CloningDataType.oOtherData.EncodingType;
            if (!_CloningDataType.EmptyCheck(_CloningDataType.oOtherData.ParrentType))
                NewDataType.oOtherData.ParrentType = _CloningDataType.oOtherData.ParrentType;
            if (!_CloningDataType.EmptyCheck(_CloningDataType.oOtherData.TagNumber))
                NewDataType.oOtherData.TagNumber = _CloningDataType.oOtherData.TagNumber;
            if (!_CloningDataType.EmptyCheck(_CloningDataType.oRange.Max))
                NewDataType.oRange.Max = _CloningDataType.oRange.Max;
            if (!_CloningDataType.EmptyCheck(_CloningDataType.oRange.Min))
                NewDataType.oRange.Min = _CloningDataType.oRange.Min;
            if (!_CloningDataType.EmptyCheck(_CloningDataType.oSize.Size))
                NewDataType.oSize.Size = _CloningDataType.oSize.Size;
            if (!_CloningDataType.EmptyCheck(_CloningDataType.oSequence.lElements.Count))
                NewDataType.oSequence.lElements = _CloningDataType.oSequence.lElements;
            return NewDataType;
        }

    }
}