﻿using System;
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
        DataType oDataType = new DataType();
        

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

        private byte[] DataTypeClassification(DataType _oDataType, List<string> _ValueToEncode, DataHub _oDataHub)
        {
            byte[] EncodedValue;

            if (_oDataType.oSequence.lElements.Count == 0)
            {
                if (_oDataType.oOtherData.ParrentType == null)
                {
                    return EncodedValue = Encode(_ValueToEncode[0], _oDataType.oOtherData.Class, _oDataType.oOtherData.TagNumber, _oDataType.oOtherData.TagNumber);
                }
                else
                {
                    if (_oDataType.oOtherData.EncodingType == "EXPLICIT")
                    {
                        oDataType = _oDataHub.FindDataTypeByName(_oDataType.oOtherData.ParrentType);
                        string ParrentClass = oDataType.oOtherData.Class;
                        int ParrentTagNumber = oDataType.oOtherData.TagNumber;
                        //Console.WriteLine(oDataType.TypeName);
                        while (oDataType.oOtherData.ParrentType != null)
                        {
                            oDataType = _oDataHub.FindDataTypeByName(oDataType.oOtherData.ParrentType);
                            //Console.WriteLine(oDataType.TypeName);
                        }
                        if (_oDataType.oOtherData.Class != null && _oDataType.oOtherData.TagNumber != 0)
                            if (ParrentClass != null && ParrentTagNumber != 0)
                                Encode(_ValueToEncode[0], _oDataType.oOtherData.Class, ParrentClass, _oDataType.oOtherData.TagNumber,
                                    ParrentTagNumber, oDataType.oOtherData.TagNumber);
                            else
                                Encode(_ValueToEncode[0], _oDataType.oOtherData.Class, "CONTEXT-SPECIFIC", _oDataType.oOtherData.TagNumber,
                                    ParrentTagNumber, oDataType.oOtherData.TagNumber);

                        else
                            if (ParrentClass != null && ParrentTagNumber != 0)
                                Encode(_ValueToEncode[0], "CONTEXT-SPECIFIC", ParrentClass, 0,
                                    ParrentTagNumber, oDataType.oOtherData.TagNumber);
                            else
                                Encode(_ValueToEncode[0], "CONTEXT-SPECIFIC", ParrentClass, 0,
                                    ParrentTagNumber, oDataType.oOtherData.TagNumber);
                    }
                    else
                    {
                        oDataType = _oDataHub.FindDataTypeByName(_oDataType.oOtherData.ParrentType);
                        //Console.WriteLine(oDataType.TypeName);
                        while (oDataType.oOtherData.ParrentType != null)
                        {
                            oDataType = _oDataHub.FindDataTypeByName(oDataType.oOtherData.ParrentType);
                            //Console.WriteLine(oDataType.TypeName);
                        }

                        if (_oDataType.oOtherData.Class != null && _oDataType.oOtherData.TagNumber != 0)
                        {
                            return EncodedValue = Encode(_ValueToEncode[0], _oDataType.oOtherData.Class, _oDataType.oOtherData.TagNumber, oDataType.oOtherData.TagNumber);
                        }
                        else if (_oDataType.oOtherData.Class == null && _oDataType.oOtherData.TagNumber != 0)
                        {
                            return EncodedValue = Encode(_ValueToEncode[0], "CONTEXT-SPECIFIC", _oDataType.oOtherData.TagNumber, oDataType.oOtherData.TagNumber);
                        }
                            
                        else
                        {
                            DataType oParrentDataType = new DataType();
                            oParrentDataType = FindParrent(_oDataHub, _oDataType.oOtherData.ParrentType);

                            if (oParrentDataType.oOtherData.Class == null && oParrentDataType.oOtherData.TagNumber != 0)
                                return EncodedValue = Encode(_ValueToEncode[0], "CONTEXT-SPECIFIC", oParrentDataType.oOtherData.TagNumber, oDataType.oOtherData.TagNumber);
                            else
                                return EncodedValue = Encode(_ValueToEncode[0], oParrentDataType.oOtherData.Class, oParrentDataType.oOtherData.TagNumber, oDataType.oOtherData.TagNumber);
                        }
                    }
                }
            }
            else
            {
                int Iterator = 0;
                List<String> Value = new List<string>();
                Value.Add(null);
                byte[] EncodedIdentifier;
                byte[] EncodedLenght;
                byte[] EncodedContents = null;

                if(_oDataType.oOtherData.ParrentType == null)
                {
                    EncodedIdentifier = EncodeIdentifier(_oDataType.oOtherData.Class, true, _oDataType.oOtherData.TagNumber);
                }
                else
                {
                    if(_oDataType.oOtherData.Class == null)
                    {
                        if(_oDataType.oOtherData.TagNumber == 0)
                        {
                            DataType oParrentDataType = new DataType();
                            oParrentDataType = FindParrent(_oDataHub, _oDataType.oOtherData.ParrentType);
                            if(oParrentDataType.oOtherData.Class != null)
                                EncodedIdentifier = EncodeIdentifier(oParrentDataType.oOtherData.Class, true, oParrentDataType.oOtherData.TagNumber);
                            else
                                EncodedIdentifier = EncodeIdentifier("CONTEXT-SPECIFIC", true, oParrentDataType.oOtherData.TagNumber);
                        }
                        else
                            EncodedIdentifier = EncodeIdentifier("CONTEXT-SPECIFIC", true, _oDataType.oOtherData.TagNumber);
                    }
                    else
                        EncodedIdentifier = EncodeIdentifier(_oDataType.oOtherData.Class, true, _oDataType.oOtherData.TagNumber);
                }

                foreach (SequenceElement oSequenceElement in _oDataType.oSequence.lElements)
                {
                    if (oSequenceElement.ElementType.oSequence.lElements.Count() == 0)
                        Value[0] = _ValueToEncode[Iterator];
                    else
                    {
                        Value = new List<string>(oSequenceElement.ElementType.oSequence.lElements.Count());
                        Value = _ValueToEncode.GetRange(Iterator, oSequenceElement.ElementType.oSequence.lElements.Count());
                        Iterator += oSequenceElement.ElementType.oSequence.lElements.Count();
                    }
                    if (Iterator == 0)
                    {
                        EncodedContents = DataTypeClassification(oSequenceElement.ElementType, Value, _oDataHub);
                    }
                    else
                        EncodedContents = EncodedContents.Concat(DataTypeClassification(oSequenceElement.ElementType, Value, _oDataHub)).ToArray();
                    Iterator++;
                }

                EncodedLenght = EncodeLenght(EncodedContents);
                PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                return EncodedValue = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
            }
            return EncodedValue = null;
        }

        private DataType FindParrent(DataHub _oDataHub, string ParrentName)
        {
            DataType _oDataType = new DataType();
            _oDataType = _oDataHub.FindDataTypeByName(ParrentName);
            if (_oDataType.oOtherData.TagNumber != 0)
                return _oDataType;
            else
                return FindParrent(_oDataHub, _oDataType.oOtherData.ParrentType);
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
                        //Console.WriteLine("Koduję se Boola!");
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        EncodedContents = EncodeBool(_ValueToEncode);
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 2:
                    {
                        //Console.WriteLine("Koduję se Integera!");
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        EncodedContents = EncodeInt(Int32.Parse(_ValueToEncode));
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 4:
                    {
                        //Console.WriteLine("Koduję se Octeta!");
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        EncodedContents = EncodeString(_ValueToEncode);
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 5:
                    {
                        //Console.WriteLine("Koduję se Nulla!");
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        EncodedContents = null;
                        EncodedLenght = EncodeLenght(new byte[0]);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        return Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                    }
                case 6:
                    {
                        //Console.WriteLine("Koduję se Objecta!");
                        EncodedIdentifier = EncodeIdentifier(_Class, false, _TagNumber);
                        return Encoded = null;
                    }         
            }
            return Encoded = null;
        }

        /*private void Encode(string _ValueToEncode, int _TagNumber, string _ParrentName)
        {
            byte[] Encoded;
            byte[] EncodedIdentifier;
            byte[] EncodedLenght;
            byte[] EncodedContents;

            switch (_ParrentName)
            {
                case "INTEGER":
                    {
                        Console.WriteLine("Koduję se ten typ jako Int!");
                        EncodedIdentifier = EncodeIdentifier("CONTEXT-SPECIFIC", false, _TagNumber);
                        EncodedContents = EncodeInt(Int32.Parse(_ValueToEncode));
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        break;
                    }
                case "OCTET STRING":
                    {
                        Console.WriteLine("Koduję se ten typ jako Octeta!");
                        EncodedIdentifier = EncodeIdentifier("CONTEXT-SPECIFIC", false, _TagNumber);
                        EncodedContents = EncodeString(_ValueToEncode);
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);
                        break; ;
                    }
                case "OBJECT IDENTIFIER":
                    {
                        Console.WriteLine("Koduję se ten typ jako Objecta!");
                        EncodedIdentifier = EncodeIdentifier("CONTEXT-SPECIFIC", false, _TagNumber);
                        break;
                    }
            }
        }*/

        private void Encode(string _ValueToEncode, string _Class, string _ParrentClass, int _TagNumber,int _ParrentTagNumber, int _OriginTagNumber)
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
                        //Console.WriteLine("Koduję se Boola!");
                        EncodedParrentIdentifier = EncodeIdentifier(_ParrentClass, false, _ParrentTagNumber);
                        EncodedParrentContents = EncodeBool(_ValueToEncode);
                        EncodedParrentLenght = EncodeLenght(EncodedParrentContents);

                        EncodedIdentifier = EncodeIdentifier(_Class, true, _TagNumber);
                        EncodedContents = EncodedParrentIdentifier.Concat(EncodedParrentLenght.Concat(EncodedParrentContents)).ToArray();
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                        break;
                    }
                case 2:
                    {
                        //Console.WriteLine("Koduję se EXPLICIT ten typ jako integer");
                        EncodedParrentIdentifier = EncodeIdentifier(_ParrentClass, false, _ParrentTagNumber);
                        EncodedParrentContents = EncodeInt(Int32.Parse(_ValueToEncode));
                        EncodedParrentLenght = EncodeLenght(EncodedParrentContents);

                        EncodedIdentifier = EncodeIdentifier(_Class, true, _TagNumber);
                        EncodedContents = EncodedParrentIdentifier.Concat(EncodedParrentLenght.Concat(EncodedParrentContents)).ToArray();
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                        break;
                    }
                case 4:
                    {
                        //Console.WriteLine("Koduję se EXPLICIT ten typ jako Octeta!");
                        EncodedParrentIdentifier = EncodeIdentifier(_ParrentClass, false, _ParrentTagNumber);
                        EncodedParrentContents = EncodeString(_ValueToEncode);
                        EncodedParrentLenght = EncodeLenght(EncodedParrentContents);

                        EncodedIdentifier = EncodeIdentifier(_Class, true, _TagNumber);
                        EncodedContents = EncodedParrentIdentifier.Concat(EncodedParrentLenght.Concat(EncodedParrentContents)).ToArray();
                        EncodedLenght = EncodeLenght(EncodedContents);
                        PrintEncodedValue(EncodedIdentifier, EncodedLenght, EncodedContents);

                        Encoded = EncodedIdentifier.Concat(EncodedLenght.Concat(EncodedContents)).ToArray();
                        break;
                    }
                case 6:
                    {
                        //Console.WriteLine("Koduję se EXPLICIT ten typ jako Objecta!");
                        EncodeIdentifier(_Class, true, _TagNumber);
                        break;
                    }
            }
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
                    if (i == EncodedIdentifier.Length - 1)
                        EncodedIdentifier[i] += 0;
                    else
                        EncodedIdentifier[i] += 128;

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
            if (_Complexity == false)
                return 0;
            else
                return 32;
        }

        private byte[] EncodeBool(string _ValueToEncode)
        {
            byte[] EncodedContents = new byte[1];

            if (_ValueToEncode == "true")
                EncodedContents[0] = 255;
            else
                EncodedContents[0] = 0;
            return EncodedContents;
        }

        private byte[] EncodeInt(int _ValueToEncode)
        {
            byte[] EncodedContents;
            string NegativeValueToEncode = null;

            if (_ValueToEncode != int.MinValue)
                EncodedContents = new byte[((Convert.ToString(Math.Abs(_ValueToEncode), 2).Length) / 8) + 1];
            else
                EncodedContents = new byte[((Convert.ToString(Math.Abs(_ValueToEncode - 1) + 1, 2).Length) / 8) + 1];


            //Console.WriteLine("Liczba to: " + Int32.Parse(Convert.ToString(Math.Abs(_ValueToEncode), 2)).ToString("0000 0000 "));
            //Console.WriteLine("ilość oktetów: " + EncodedContents.Length);

            int OctetLength = 0;
            if (_ValueToEncode != int.MinValue)
                OctetLength = Convert.ToString(Math.Abs(_ValueToEncode), 2).Length;
            else
                OctetLength = Convert.ToString(Math.Abs(_ValueToEncode - 1) + 1, 2).Length;

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
            MatchCollection OIDS = Regex.Matches(@"\S+", _ValueToEncode, RegexOptions.Singleline | RegexOptions.Compiled);

            foreach (Match _match in OIDS)
            {
                //_match.Groups[0].Value;

            }
            return EncodedContents;
        }

        private byte[] EncodeLenght(byte[] _EncodedContents)
        {
            byte[] _EncodedLength = new byte[(_EncodedContents.Count() / 255) + 1];

            _EncodedLength[0] = Convert.ToByte(_EncodedContents.Count());           

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
