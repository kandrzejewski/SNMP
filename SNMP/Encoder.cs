using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SNMP
{
    class Encoder
    {
        private DataType oDataType = new DataType();
        public int iteration = 0;
        
        public byte[] EncodingInit(EncoderData _oEncoderData, DataHub _oDataHub)
        {   
            iteration = 0;
            Console.Clear();
            if (_oEncoderData.bCanEncode)
            {
                _oEncoderData.PresentData();
                Console.WriteLine("\nEncoding...");
                return DataTypeClassification(_oEncoderData._oDataType, _oEncoderData.ValueToEncode, _oDataHub);
            }
            else
            {
                _oEncoderData.PresentData();
                Console.WriteLine("\nReturning to menu.");
                return null;
            }
        }

        private byte[] DataTypeClassification(DataType _oDataType, List<string> _ValueToEncode, DataHub _oDataHub)
        {
            int iTagNumber = 0;
            string sClass = null;

            if (_oDataType.oSequence.lElements.Count == 0)
            {
                if (_oDataType.oOtherData.ParrentType == null)
                {
                    return _oDataType.oOtherData.Class != null ?
                        Encode(_ValueToEncode[iteration], _oDataType.oOtherData.Class, _oDataType.oOtherData.TagNumber, _oDataType.oOtherData.TagNumber) :
                        Encode(_ValueToEncode[iteration], "CONTEXT-SPECIFIC", _oDataType.oOtherData.TagNumber, _oDataType.oOtherData.TagNumber);
                }
                else
                {                   
                    if (_oDataType.oOtherData.EncodingType == "EXPLICIT")
                    {
                        int iParrentTagNumber = 0;
                        string sParrentClass = null;

                        iTagNumber = ClassificationIdentifier(_oDataHub, _oDataType).Key;
                        sClass = ClassificationIdentifier(_oDataHub, _oDataType).Value;

                        oDataType = _oDataHub.FindDataTypeByName(_oDataType.oOtherData.ParrentType);
                        iParrentTagNumber = ClassificationIdentifier(_oDataHub, oDataType).Key;
                        sParrentClass = ClassificationIdentifier(_oDataHub, oDataType).Value;
                        while (oDataType.oOtherData.ParrentType != null)
                            oDataType = _oDataHub.FindDataTypeByName(oDataType.oOtherData.ParrentType);

                        return Encode(_ValueToEncode[iteration], sClass, sParrentClass, iTagNumber, iParrentTagNumber, oDataType.oOtherData.TagNumber);               
                    }
                    else
                    {
                        iTagNumber = ClassificationIdentifier(_oDataHub, _oDataType).Key;
                        sClass = ClassificationIdentifier(_oDataHub, _oDataType).Value;

                        oDataType = _oDataHub.FindDataTypeByName(_oDataType.oOtherData.ParrentType);
                        while (oDataType.oOtherData.ParrentType != null)
                            oDataType = _oDataHub.FindDataTypeByName(oDataType.oOtherData.ParrentType);

                        return Encode(_ValueToEncode[iteration], sClass, iTagNumber, oDataType.oOtherData.TagNumber);
                    }
                }
            }
            else
            {  
                byte[] EncodedIdentifier;
                byte[] EncodedLenght;
                byte[] EncodedContents = new byte[0];

                if(_oDataType.oOtherData.ParrentType == null)
                {
                    EncodedIdentifier = EncodeIdentifier(_oDataType.oOtherData.Class, true, _oDataType.oOtherData.TagNumber);
                }
                else
                {
                    iTagNumber = ClassificationIdentifier(_oDataHub, _oDataType).Key;
                    sClass = ClassificationIdentifier(_oDataHub, _oDataType).Value;

                    EncodedIdentifier = EncodeIdentifier(sClass, true, iTagNumber);
                }

                foreach (SequenceElement oSequenceElement in _oDataType.oSequence.lElements)
                {
                    if (iteration == 0)
                        EncodedContents = DataTypeClassification(oSequenceElement.ElementType, _ValueToEncode, _oDataHub);
                    else
                        EncodedContents = EncodedContents.Concat(DataTypeClassification(oSequenceElement.ElementType, _ValueToEncode, _oDataHub)).ToArray();
                    if(oSequenceElement != _oDataType.oSequence.lElements.Last())
                        iteration++;
                }

                EncodedLenght = EncodeLenght(EncodedContents);
                PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                return EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
            }
        }

        private KeyValuePair<int, string> ClassificationIdentifier(DataHub _oDataHub, DataType _oDataType)
        {
            int _iTagNumber;
            string _sClass;

            if (_oDataType.oOtherData.TagNumber != 0)
            {
                _iTagNumber = _oDataType.oOtherData.TagNumber;
                _sClass = _oDataType.oOtherData.Class != null ?
                    _oDataType.oOtherData.Class : "CONTEXT-SPECIFIC";
            }
            else
            {
                _iTagNumber = FindParrent(_oDataHub, _oDataType.oOtherData.ParrentType).oOtherData.TagNumber;
                _sClass = FindParrent(_oDataHub, _oDataType.oOtherData.ParrentType).oOtherData.Class != null ?
                   FindParrent(_oDataHub, _oDataType.oOtherData.ParrentType).oOtherData.Class : "CONTEXT-SPECIFIC";
            }
            return new KeyValuePair<int, string>(_iTagNumber, _sClass);
        }

        private DataType FindParrent(DataHub _oDataHub, string ParrentName)
        {
            DataType _oDataType = new DataType();
            _oDataType = _oDataHub.FindDataTypeByName(ParrentName);
            return _oDataType.oOtherData.TagNumber != 0 ? 
                _oDataType : FindParrent(_oDataHub, _oDataType.oOtherData.ParrentType);
        }

        private byte[] Encode(string _ValueToEncode, string _Class, int _TagNumber, int _OriginTagNumber)
        {
            byte[] Encoded;
            byte[] EncodedIdentifier;
            byte[] EncodedLenght;
            byte[] EncodedContents;
            switch (_OriginTagNumber)
            {
                case 1:
                    {
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        EncodedContents = EncodeBool(_ValueToEncode);
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 2:
                    {
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        EncodedContents = EncodeInt(Int32.Parse(_ValueToEncode));
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 4:
                    {
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        EncodedContents = EncodeString(_ValueToEncode);
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 5:
                    {
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        EncodedContents = null;
                        EncodedLenght = EncodeLenght(new byte[0]);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 6:
                    {
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        EncodedContents = EncodeObjectIdentifier(_ValueToEncode);
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }         
            }
            return Encoded = null;
        }

        private byte[] Encode(string _ValueToEncode, string _Class, string _ParrentClass, int _TagNumber,int _ParrentTagNumber, int _OriginTagNumber)
        {
            byte[] Encoded;
            byte[] EncodedIdentifier;
            byte[] EncodedLenght;
            byte[] EncodedContents;
            byte[] EncodedParrentIdentifier;
            byte[] EncodedParrentLenght;
            byte[] EncodedParrentContents;

            switch (_OriginTagNumber)
            {
                case 1:
                    {
                        EncodedParrentIdentifier = EncodeIdentifier(_ParrentClass, false, _ParrentTagNumber);
                        EncodedParrentContents = EncodeBool(_ValueToEncode);
                        EncodedParrentLenght = EncodeLenght(EncodedParrentContents);

                        EncodedIdentifier = EncodeIdentifier(_Class, true, _TagNumber);
                        EncodedContents = EncodedParrentIdentifier.Concat(EncodedParrentLenght.Concat(EncodedParrentContents)).ToArray();
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 2:
                    {
                        EncodedParrentIdentifier = EncodeIdentifier(_ParrentClass, false, _ParrentTagNumber);
                        EncodedParrentContents = EncodeInt(Int32.Parse(_ValueToEncode));
                        EncodedParrentLenght = EncodeLenght(EncodedParrentContents);

                        EncodedIdentifier = EncodeIdentifier(_Class, true, _TagNumber);
                        EncodedContents = EncodedParrentIdentifier.Concat(EncodedParrentLenght.Concat(EncodedParrentContents)).ToArray();
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 4:
                    {
                        EncodedParrentIdentifier = EncodeIdentifier(_ParrentClass, false, _ParrentTagNumber);
                        EncodedParrentContents = EncodeString(_ValueToEncode);
                        EncodedParrentLenght = EncodeLenght(EncodedParrentContents);

                        EncodedIdentifier = EncodeIdentifier(_Class, true, _TagNumber);
                        EncodedContents = EncodedParrentIdentifier.Concat(EncodedParrentLenght.Concat(EncodedParrentContents)).ToArray();
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 6:
                    {
                        EncodedParrentIdentifier = EncodeIdentifier(_ParrentClass, false, _ParrentTagNumber);
                        EncodedParrentContents = EncodeObjectIdentifier(_ValueToEncode);
                        EncodedParrentLenght = EncodeLenght(EncodedParrentContents);

                        EncodedIdentifier = EncodeIdentifier(_Class, true, _TagNumber);
                        EncodedContents = EncodedParrentIdentifier.Concat(EncodedParrentLenght.Concat(EncodedParrentContents)).ToArray();
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
            }
            return Encoded = null;
        }

        private byte[] EncodeIdentifier(string _Class, bool _Complexity, int _TagNumber)
        {
            byte[] EncodedIdentifier;

            if (_TagNumber < 31)
            {
                EncodedIdentifier = new byte[1];
                EncodedIdentifier[0] = Convert.ToByte(Convert.ToByte(_TagNumber) + EncodeClass(_Class) + EncodeComplexity(_Complexity));
            }
            else
            {
                EncodedIdentifier = new byte[((Convert.ToString(_TagNumber, 2).Length) / 8) + 2];
                //Console.WriteLine("Ilość oktetów: " + ((Convert.ToString(_TagNumber, 2).Length) / 8));
                int OctetLength = 0;
                OctetLength = Convert.ToString(_TagNumber, 2).Length;

                EncodedIdentifier[0] = Convert.ToByte(31 + EncodeClass(_Class) + EncodeComplexity(_Complexity));
                for (byte i = 1; i < EncodedIdentifier.Length; i++)
                {
                    EncodedIdentifier[i] += i == EncodedIdentifier.Length - 1 ? 
                        Convert.ToByte(0) : Convert.ToByte(128);

                    if (OctetLength >= 7)
                    {
                        OctetLength -= 7;
                        //Console.WriteLine("zawartość TagNumber {0}: " + Convert.ToString(_TagNumber, 2).Substring(7 * (i - 1), 7), i);
                        EncodedIdentifier[EncodedIdentifier.Length - i] += Convert.ToByte(Convert.ToString(_TagNumber, 2).Substring(OctetLength, 7), 2);
                    }
                    else
                    {
                        //Console.WriteLine("zawartość TagNumber {0}: " + Convert.ToString(_TagNumber, 2).Substring(7 * (i - 1)), i);
                        EncodedIdentifier[EncodedIdentifier.Length - i] += Convert.ToByte(Convert.ToString(_TagNumber, 2).Substring(0, OctetLength), 2);
                    }
                }
            }
            return EncodedIdentifier;
        }

        private byte EncodeClass(string _Class)
        {
            switch (_Class)
            {
                case "UNIVERSAL":
                    return 0;
                case "APPLICATION":
                    return 64;
                case "CONTEXT-SPECIFIC":
                    return 128;
                case "PRIVATE":
                    return 192;
                default:
                    return 0;
            }
        }

        private byte EncodeComplexity(bool _Complexity)
        {
            return _Complexity == false? Convert.ToByte(0) : Convert.ToByte(32);
        }

        private byte[] EncodeBool(string _ValueToEncode)
        {
            byte[] EncodedContents = new byte[1];
            EncodedContents[0] = _ValueToEncode == "true" ? Convert.ToByte(255) : Convert.ToByte(0);
            return EncodedContents;
        }

        private byte[] EncodeInt(int _ValueToEncode)
        {
            byte[] EncodedContents;
            string NegativeValueToEncode = null;
            int OctetLength = 0;

            EncodedContents = _ValueToEncode != int.MinValue ? new byte[((Convert.ToString(Math.Abs(_ValueToEncode), 2).Length) / 8) + 1] :
                new byte[((Convert.ToString(Math.Abs(_ValueToEncode - 1) + 1, 2).Length) / 8) + 1];

            //Console.WriteLine("Liczba to: " + Int32.Parse(Convert.ToString(Math.Abs(_ValueToEncode), 2)).ToString("0000 0000 "));
            //Console.WriteLine("ilość oktetów: " + EncodedContents.Length);
            OctetLength = _ValueToEncode != int.MinValue ? Convert.ToString(Math.Abs(_ValueToEncode), 2).Length :
                Convert.ToString(Math.Abs(_ValueToEncode - 1) + 1, 2).Length;

            if (_ValueToEncode >= 0)
            {
                return EncodeNumber(_ValueToEncode, OctetLength, EncodedContents);
            }
            else
            {
                foreach (char Sign in Convert.ToString(Math.Abs(_ValueToEncode - 1), 2))
                {
                    if (Sign == '1')
                        NegativeValueToEncode += "0";
                    else
                        NegativeValueToEncode += "1";
                }
                //Console.WriteLine("Zanegowana liczba to: " + NegativeValueToEncode);
                //Console.WriteLine("Uzupełnienie U2: " + new String('1', 8 - (NegativeValueToEncode.Length % 8)));
                if (NegativeValueToEncode.Length % 8 == Convert.ToString(Math.Abs(_ValueToEncode - 1) + 1, 2).Length % 8)
                    EncodedContents[0] = Convert.ToByte(new String('1', 8 - (NegativeValueToEncode.Length % 8)) + new String('0', NegativeValueToEncode.Length % 8), 2);
                else
                {  
                    if (NegativeValueToEncode.Length % 8 == 7)
                    {
                        EncodedContents[0] = Convert.ToByte(new String('1', 8), 2);
                        EncodedContents[1] = Convert.ToByte(new String('1', 8 - (NegativeValueToEncode.Length % 8)) + new String('0', NegativeValueToEncode.Length % 8), 2);
                    }
                    else
                    {
                        EncodedContents[0] = Convert.ToByte(new String('1', 8 - (NegativeValueToEncode.Length % 8)) + new String('0', NegativeValueToEncode.Length % 8), 2);
                    }
                }
                return EncodeNumber(NegativeValueToEncode, OctetLength, EncodedContents);
            } 
        }

        private byte[] EncodeNumber(int _ValueToEncode, int _OctetLength, byte[] EncodedContents)
        {
            byte[] _EncodedContents = EncodedContents;
            for (int i = 0; i < _EncodedContents.Length; i++)
            {
                //Console.WriteLine("Długość oktetu:" + _OctetLength);

                if (_OctetLength >= 8)
                {
                    _OctetLength -= 8;
                    _EncodedContents[_EncodedContents.Length - i - 1] += Convert.ToByte(Convert.ToString(_ValueToEncode, 2).Substring(_OctetLength, 8), 2);
                }
                else
                {
                    if (_OctetLength == 0)
                    {
                        _EncodedContents[_EncodedContents.Length - i - 1] = 0;
                    }
                    else
                    {
                        //Console.WriteLine("zawartość TagNumber {0}: " + Convert.ToString(_TagNumber, 2).Substring(7 * (i - 1)), i);
                        _EncodedContents[_EncodedContents.Length - i - 1] += Convert.ToByte(Convert.ToString(_ValueToEncode, 2).Substring(0, _OctetLength), 2);
                    }
                }
            }
            return _EncodedContents;
        }

        private byte[] EncodeNumber(string _ValueToEncode, int _OctetLength, byte[] EncodedContents)
        {
            byte[] _EncodedContents = EncodedContents;
            for (int i = 0; i < _EncodedContents.Length; i++)
            {
                //Console.WriteLine("Długość oktetu:" + _OctetLength);

                if (_OctetLength >= 8)
                {
                    _OctetLength -= 8;
                    if (Convert.ToUInt64(_ValueToEncode, 2) != 0)
                    {
                        _EncodedContents[_EncodedContents.Length - i - 1] += Convert.ToByte(_ValueToEncode.Substring(_OctetLength, 8), 2);
                    }
                }
                else
                {
                    if (_OctetLength == 0)
                    {
                        _EncodedContents[_EncodedContents.Length - i - 1] += 0;
                    }
                    else
                    {
                        //Console.WriteLine("zawartość TagNumber {0}: " + Convert.ToString(_TagNumber, 2).Substring(7 * (i - 1)), i);
                        if (Convert.ToByte(_ValueToEncode, 2) != 0)
                            _EncodedContents[_EncodedContents.Length - i - 1] += Convert.ToByte(_ValueToEncode.Substring(0, _OctetLength), 2);
                    }
                }
            }
            return _EncodedContents;
        }

        private byte[] EncodeString(string _ValueToEncode)
        {
            byte[] EncodedContents = new byte[_ValueToEncode.Count()];
            byte[] AsciiValues = System.Text.ASCIIEncoding.ASCII.GetBytes(_ValueToEncode);
            for (int i = 0; i < EncodedContents.Count(); i++)
            {
                //Console.WriteLine("Kod int znaku:" + AsciiValues[i]);
                EncodedContents[i] = EncodeInt(AsciiValues[i])[0];
            }
            return EncodedContents;
        }

        private byte[] EncodeObjectIdentifier(string _ValueToEncode)
        {
            byte[] EncodedContents;
            Regex RegexOID = new Regex(@"\S+", RegexOptions.Singleline | RegexOptions.Compiled);
            MatchCollection OIDs = RegexOID.Matches(_ValueToEncode);

            EncodedContents = new byte[OIDs.Count > 1 ? OIDs.Count - 1 : OIDs.Count];

            for (int i = 0; i < OIDs.Count; i++)
            {
                if (i == 0)
                    EncodedContents[0] = Convert.ToByte(40 * Convert.ToByte(OIDs[i].Groups[0].Value));
                else if (i == 1)
                    EncodedContents[0] += Convert.ToByte(OIDs[i].Groups[0].Value);
                else
                    EncodedContents[i - 1] = Convert.ToByte(OIDs[i].Groups[0].Value);
            }
            return EncodedContents;
        }

        private byte[] EncodeLenght(byte[] _EncodedContents)
        {
            byte[] _EncodedLength = new byte[(_EncodedContents.Count() / 255) + 1];
            _EncodedLength[0] = Convert.ToByte(_EncodedContents.Count());         
            //Tutaj w razie potrzeby trzeba dokończyć dla długości większej niż 8bitów
            return _EncodedLength;
        }

        private void PrintEncodedValue(byte[] _EncodedIdentifier, byte[] _EncodedLength, byte[] _EncodedContents)
        {
            Console.WriteLine("\nEncoded Value: ");
            foreach (byte Value in _EncodedIdentifier)
            {
                Console.Write(Int32.Parse(Convert.ToString(Value, 2)).ToString("0000 0000 "));
            }
            Console.Write("| ");
            Console.Write(Int32.Parse(Convert.ToString(_EncodedLength[0], 2)).ToString("0000 0000 "));
            if (_EncodedContents != null)
            {
                Console.Write("| ");
                foreach (byte Value in _EncodedContents)
                {
                    Console.Write(Int32.Parse(Convert.ToString(Value, 2)).ToString("0000 0000 "));
                }
            }
            Console.WriteLine();
        }
    }
}
