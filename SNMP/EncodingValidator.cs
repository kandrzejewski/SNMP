using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace SNMP
{
    class EncodingValidator : DataHub
    {
        private EncoderData oResult = new EncoderData();
        private DataType oDataType = new DataType();

        public EncoderData ValidateAnyDataType()
        {
            string sNameOfType;
            Console.Write("Enter a name of Type: ");
            sNameOfType = Console.ReadLine();
            if(sNameOfType != "SEQUENCE")
            {
                oDataType = FindDataTypeByName(sNameOfType);
                if (oDataType != null)
                {
                    oResult = Validate("\n\nFound Data Type " +
                    oDataType.TypeName, oDataType, 0);
                }
                else
                {
                    Console.WriteLine("Entered Data Type not found!");
                    oResult.bCanEncode = false;
                }
            }
            else
            {
                Console.WriteLine("\n\nCreate a sequence to encode.");
                oDataType = CreateSequence();
                oResult = Validate("\n\nCreated Sequence: " +
                    oDataType.TypeName, oDataType, 0);
            }

            return oResult;
        }

        private DataType CreateSequence()
        {
            int iSequenceIndex = 0;
            DataType _oDataType = new DataType();
            char SequenceComplete = 'Y';
            while (SequenceComplete == 'Y')
            {
                Console.Write("\nEnter a {0} type name: ", iSequenceIndex++);
                _oDataType.oSequence.lElements.Add(new SequenceElement());
                _oDataType.oSequence.lElements.Last().ElementName = iSequenceIndex.ToString();
                _oDataType.oSequence.lElements.Last().ElementType = FindDataTypeByName(Console.ReadLine());
                Console.WriteLine("\n\nNext data type in sequece? (Y/N)");
                Console.Write("=>");
                if (Console.ReadKey().KeyChar == 'N' || Console.ReadKey().KeyChar == 'n')
                    SequenceComplete = 'N';
            }
            return _oDataType;
        }



        public EncoderData Validate(string _title, DataType _oDataType, byte iteration)
        {
            if (_oDataType != null)
            {
                if (iteration == 0)
                {
                    oResult.ValueToEncode.Add("");
                    oResult._oDataType = _oDataType;
                }
                Console.WriteLine(_title);
                Console.WriteLine("────────────────────────────────────────────────────");
                _oDataType.PresentData(1);
                Console.WriteLine("────────────────────────────────────────────────────");
                Console.ForegroundColor = ConsoleColor.Gray;
                if (_oDataType.TypeName == "INTEGER" || _oDataType.oOtherData.ParrentType == "INTEGER")
                {
                    oResult.bCanEncode = IntegerValidation(_oDataType, iteration);
                    return oResult;
                }
                else if (_oDataType.TypeName == "OCTET STRING" || _oDataType.oOtherData.ParrentType == "OCTET STRING")
                {
                    oResult.bCanEncode = OctetStringValidation(_oDataType, iteration);
                    return oResult;
                }
                else if (_oDataType.TypeName == "OBJECT IDENTIFIER" || _oDataType.oOtherData.ParrentType == "OBJECT IDENTIFIER")
                {
                    oResult.bCanEncode = ObjectIdentifierValidation(_oDataType, iteration);
                    return oResult;
                }
                else if (_oDataType.oSequence.lElements.Count() == 0)
                {
                    if (!_oDataType.EmptyCheck(_oDataType.oOtherData.ParrentType))
                    {
                        foreach (DataType _DataType in DataHub.lDataType)
                        {
                            if (_oDataType.oOtherData.ParrentType == _DataType.TypeName)
                                if (Validate("Parrent of Data Type of Object " + _oDataType.TypeName, _DataType, iteration).bCanEncode)
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
                        if (_SequenceElement.ElementType != null)
                            oResult.ValueToEncode.Add("");
                            iteration++;
                            if (!Validate("\n\n" + (_oDataType.oSequence.lElements.IndexOf(_SequenceElement) + 1) 
                                + " element of Sequence:", _SequenceElement.ElementType, iteration).bCanEncode)
                            {
                                oResult.bCanEncode = false;
                                return oResult;
                            }
                                
                    }
                    oResult.bCanEncode = true;
                    return oResult;
                }
                Console.WriteLine("\n\nParameters of Data Type is not recognized!");
                oResult.bCanEncode = false;
                return oResult;
            }
            else
            {
                Console.WriteLine("\n\nObject doesn't have a Data Type!");
                oResult.bCanEncode = false;
                return oResult;
            }

        }      

        private bool IntegerValidation(DataType _oDataType, byte number)
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
            oResult.ValueToEncode[number] = Console.ReadLine();
            bConversionStatus = Int64.TryParse(oResult.ValueToEncode[number], out iValueToEncode);

            if (bConversionStatus)
            {
                if (iValueToEncode < _oDataType.oRange.Min)
                {
                    Console.WriteLine("\nPodana liczba jest poza zakresem. Minimalna wartość to " + _oDataType.oRange.Min);
                    return false;
                }

                if (!_oDataType.EmptyCheck(_oDataType.oRange.Max))
                    if (iValueToEncode > _oDataType.oRange.Max)
                    {
                        Console.WriteLine("\nPodana liczba jest poza zakresem. Maksymalna wartość to " + _oDataType.oRange.Max);
                        return false;
                    }
            }
            else
            {
                Console.WriteLine("\nValue is incorrect");
                return false;
            }
            return true;
        }

        private bool IntegerValidation(DataType _oDataType, long _Value)
        {
            if (_Value < _oDataType.oRange.Min)
            {
                Console.WriteLine("\nPodana liczba jest poza zakresem. Minimalna wartość to " + _oDataType.oRange.Min);
                return false;
            }

            if (!_oDataType.EmptyCheck(_oDataType.oRange.Max))
                if (_Value > _oDataType.oRange.Max)
                {
                    Console.WriteLine("\nPodana liczba jest poza zakresem. Maksymalna wartość to " + _oDataType.oRange.Max);
                    return false;
                }
            return true;
        }

        private bool OctetStringValidation(DataType _oDataType, byte number)
        {   
            if(_oDataType.oSize.Size !=0)
                Console.WriteLine("Enter a Value\n({0}\nRange 0..{1} bytes)\n", _oDataType.TypeName,
                     _oDataType.oSize.Size);
            else
                Console.WriteLine("Enter a Value:\n({0})\n", _oDataType.TypeName);

            Console.Write("=>");
            oResult.ValueToEncode[number] = Console.ReadLine();
            if (!_oDataType.EmptyCheck(_oDataType.oSize.Size))
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(oResult.ValueToEncode[number]) > _oDataType.oSize.Size)
                {
                    Console.WriteLine("\nPodana liczba jest poza zakresem. Maksymalna wielkość to {0} bit!", _oDataType.oSize.Size);
                    return false;
                }
            if (!_oDataType.EmptyCheck(_oDataType.oRange.Max))
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(oResult.ValueToEncode[number]) > _oDataType.oRange.Max / 8)
                {
                    Console.WriteLine("\nPodana liczba jest poza zakresem. Maksymalna wielkość to {0} bit!", (_oDataType.oRange.Max / 8) + 1);
                    return false;
                }
            return true;
        }

        private bool OctetStringValidation(DataType _oDataType, string _value)
        {     
            if (!_oDataType.EmptyCheck(_oDataType.oSize.Size))
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(_value) > _oDataType.oSize.Size)
                {
                    Console.WriteLine("\nPodana liczba jest poza zakresem. Maksymalna wielkość to {0} bit!", _oDataType.oSize.Size);
                    return false;
                }
            if (!_oDataType.EmptyCheck(_oDataType.oRange.Max))
                if (System.Text.ASCIIEncoding.ASCII.GetByteCount(_value) > _oDataType.oRange.Max/8)
                {
                    Console.WriteLine("\nPodana liczba jest poza zakresem. Maksymalna wielkość to {0} bit!", (_oDataType.oRange.Max / 8)+1);
                    return false;
                }
                return true;
        }

        private bool ObjectIdentifierValidation(DataType _oDataType, byte number)
        {
            Console.WriteLine("Enter a Value\n(Range 0..N\nFormat: OID1 OID2 ... OIDn)");
            Console.Write("=>");
            oResult.ValueToEncode[number] = Console.ReadLine();
            Regex RegexOID = new Regex(@"\S+", RegexOptions.Singleline | RegexOptions.Compiled);
            MatchCollection OIDS = RegexOID.Matches(oResult.ValueToEncode[number]);
            int _iOID;
            foreach (Match _match in OIDS)
            {
                if (Int32.TryParse(_match.Value, out _iOID))
                {
                    if (_iOID < 0)
                    {
                        Console.WriteLine("\nA value {0} is less than 0. OID must be a positive value!", _match.Value);
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("\nA value {0} is incorrect. OID must be a positive integer value!", _match.Value);
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
                        Console.WriteLine("A value {0} is less than 0. OID must be a positive value!", _OID);
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("A value {0} is incorrect. OID must be a positive integer value!", _OID);
                    return false;
                }
            }
            return true;
        }
    }
}