using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMP
{
    class Decoder
    {
        List<DataType> lDecodedDataTypes = new List<DataType>();
        DataHub oDataHub;

        public void DecoderInit(string sDataToDecode, DataHub _oDataHub)
        {
            oDataHub = _oDataHub;

            string TempDecode = HexToBin("3029020100040861646d696e5f7277a01a0202759b020100020100300e300c06082b060102010104000500");
            Console.WriteLine("Wartość do zdekodowania: " + sDataToDecode); 
            Console.WriteLine(TempDecode);
            Decode(TempDecode).Item1.PresentData(0);
        }

        private Tuple<DecoderData, int> Decode(string _ValueToDecode)
        {
            //Console.WriteLine("Wywołano mnie!");
            //Console.WriteLine("Decode: " + _ValueToDecode);
            DecoderData _DecodedDataType = new DecoderData();
            int IdentifierEnd;

            var Identifier = DecodeIdentifier(_ValueToDecode, out IdentifierEnd);
            var ParrentData = FindParrent(Identifier.Item1, Identifier.Item3);

            _DecodedDataType.Class = Identifier != null ? Identifier.Item1 : null;
            _DecodedDataType.Complexity = Identifier != null ? Identifier.Item2 : null;
            _DecodedDataType.TagNumber = Identifier != null ? Identifier.Item3 : new Nullable<int>();
            _DecodedDataType.Length = StringBinToInt(_ValueToDecode.Substring(IdentifierEnd, 8));
            _DecodedDataType.oParrentDataType = ParrentData != null? ParrentData.Item1 : null;
            try
            {
                if (_DecodedDataType.Class == "CONTEXT-SPECIFIC" && (_DecodedDataType.TagNumber == 0 || _DecodedDataType.TagNumber == 1 || _DecodedDataType.TagNumber == 2))
                {
                    _DecodedDataType.Value = null;
                    DecodeSequence(_ValueToDecode.Substring(IdentifierEnd + 8), _DecodedDataType.Length, out _DecodedDataType.lChildrens);
                }
                else if (ParrentData.Item2 != "SEQUENCE")
                    try
                    {
                        _DecodedDataType.Value = DecodeContents(ParrentData.Item2, _ValueToDecode.Substring(IdentifierEnd + 8, 8 * _DecodedDataType.Length));
                    }
                    catch (System.ArgumentOutOfRangeException e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\nError: " + e.Message + "\n\n");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                else
                {
                    _DecodedDataType.Value = null;
                    DecodeSequence(_ValueToDecode.Substring(IdentifierEnd + 8), _DecodedDataType.Length, out _DecodedDataType.lChildrens);
                }
            }
            catch(System.NullReferenceException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\nError: Typ danych o Tagu {0} i Klasie {1} nie istnieje. Nie można zdekodować watości!\n\n", Identifier.Item3, Identifier.Item1);       
                Console.ForegroundColor = ConsoleColor.Gray;
                _DecodedDataType.Value = DecodeContents(ParrentData.Item2, _ValueToDecode.Substring(IdentifierEnd + 8, 8 * _DecodedDataType.Length));
            }
            //_DecodedDataType.PresentData(0);
            return Tuple.Create(_DecodedDataType, IdentifierEnd);
        }

        private Tuple<string,string, int> DecodeIdentifier(string _ValueToDecode, out int TagEnd)
        {
            TagEnd = 8;

            if(_ValueToDecode.Substring(3, 5) == "11111")
            {
                string sLongTag = string.Empty;
           
                sLongTag += _ValueToDecode.Substring(TagEnd + 1, 7);
                while (_ValueToDecode[TagEnd] != '0')
                {
                    TagEnd += 8;
                    sLongTag += _ValueToDecode.Substring(TagEnd + 1, 7);
                }
                TagEnd += 8;
                return Tuple.Create(DecodeClass(Convert.ToByte(_ValueToDecode.Substring(0, 2), 2)),
                    DecodeComplexity(_ValueToDecode[2]), StringBinToInt(sLongTag));
            }
            else
                return Tuple.Create(DecodeClass(Convert.ToByte(_ValueToDecode.Substring(0, 2), 2)),
                    DecodeComplexity(_ValueToDecode[2]), StringBinToInt(_ValueToDecode.Substring(3, 5)));
        }

        private string DecodeClass(byte _Class)
        {
            switch (_Class)
            {
                case 0:
                    return "UNIVERSAL";
                case 1:
                    return "APPLICATION";
                case 2:
                    return "CONTEXT-SPECIFIC";
                case 3:
                    return "PRIVATE";
                default:
                    return null;
            }
        }

        private string DecodeComplexity(char _Complexity)
        {
            return _Complexity == '0' ? "Primitive" : "Constructed";
        }

        private dynamic DecodeContents(string _Type, string _Value)
        {
            switch (_Type)
            {
                case "BOOLEAN":
                    return DecodeBoolean(_Value);
                case "INTEGER":
                    return DecodeInteger(_Value);
                case "OCTET STRING":
                    return DecodeOctetString(_Value);
                case "NULL":
                    return null;
                case "OBJECT IDENTIFIER":
                    return DecodeObjectIdentifier(_Value);
                default:
                    return null;
            }
        }

        private bool DecodeBoolean(string _ValueToDecode)
        {
            return _ValueToDecode == "11111111" ? true : false;
        }

        private int DecodeInteger(string _ValueToDecode)
        {
            int DecodedInt = 0;
            for (int i = 0; i < _ValueToDecode.Count(); i++)
            {
                DecodedInt += i == 0 
                    ? _ValueToDecode[i] == '1' 
                        ? Convert.ToInt32(-Math.Pow(2, _ValueToDecode.Count() - i - 1)) : 0 
                    : _ValueToDecode[i] == '1' 
                        ? Convert.ToInt32(Math.Pow(2, _ValueToDecode.Count() - i - 1)) : 0;
            }
            return DecodedInt;
        }

        private string DecodeOctetString(string _ValueToDecode)
        {
            string DecodedOctetString = string.Empty;
            for(int i = 0; i < _ValueToDecode.Count() / 8; i++)
            {
                DecodedOctetString += Char.ConvertFromUtf32(Convert.ToByte(_ValueToDecode.Substring(i * 8, 8),2));
            }
            return DecodedOctetString;
        }

        private string DecodeObjectIdentifier (string _ValueToDecode)
        {
            string DecodedObjectIdenfitier = "1.";
            DecodedObjectIdenfitier += Convert.ToByte(_ValueToDecode.Substring(0, 8), 2) - 40 + ".";
            for(int i = 1; i < _ValueToDecode.Count() / 8; i++)
            {
                DecodedObjectIdenfitier += Convert.ToByte(_ValueToDecode.Substring(i * 8, 8), 2) + ".";
            }
            DecodedObjectIdenfitier = DecodedObjectIdenfitier.Remove(DecodedObjectIdenfitier.Count() - 1);
            return DecodedObjectIdenfitier;
        }

        private void DecodeSequence (string _ValueToDecode, int _SequenceLength,  out List<DecoderData> _lChildrens)
        {
            int CheckSequence = _SequenceLength*8;
            _lChildrens = new List<DecoderData>();
            var _DecoderData = Decode(_ValueToDecode);
            _lChildrens.Add(_DecoderData.Item1);
            CheckSequence -= (_lChildrens.Last().Length * 8 + _DecoderData.Item2 + 8);
            while(CheckSequence != 0)
            {
                _DecoderData = Decode(_ValueToDecode.Substring(_SequenceLength*8 - CheckSequence));
                _lChildrens.Add(_DecoderData.Item1);
                CheckSequence -= _lChildrens.Last().Length * 8 + _DecoderData.Item2 + 8;
            }
        }

        private Tuple<DataType, string> FindParrent(string _Class, int _TagNumber)
        {
            string TypeName;
            var ParrentDataType = oDataHub.FindDataTypeByTag(_Class, _TagNumber);
            if(ParrentDataType != null)
            {
                if (ParrentDataType.oOtherData.ParrentType == null)
                    TypeName = ParrentDataType.TypeName;
                else
                {
                    var OriginParrent = oDataHub.FindDataTypeByName(ParrentDataType.oOtherData.ParrentType);
                    while (OriginParrent.oOtherData.ParrentType != null)
                        OriginParrent = oDataHub.FindDataTypeByName(OriginParrent.oOtherData.ParrentType);
                    TypeName = OriginParrent.TypeName;
                }
                return Tuple.Create(ParrentDataType, TypeName);
            }
            return null;
        }







        private int StringBinToInt(string _ValueToDecode)
        {
            return Convert.ToInt32(_ValueToDecode, 2);
        }

        private byte[] BinaryToByteArray(string _sDataToDecode)
        {
            Console.WriteLine("Dlugosc stringa:" + _sDataToDecode.Count());
            byte[] Value = new byte[_sDataToDecode.Count() % 8 != 0 ? 
                _sDataToDecode.Count() / 8 + 1 : _sDataToDecode.Count() / 8];
            for(int i = 0; i < Value.Count(); i++)
            {
                try
                {
                    Value[i] = Convert.ToByte(_sDataToDecode.Substring(i * 8, 8), 2);
                }
                catch (ArgumentOutOfRangeException outOfRange)
                {
                    Console.WriteLine("Error: " + outOfRange.Message);
                    Value = Value.Take(Value.Count() - 1).ToArray();
                }
            }

            return Value;
        }

        private byte[] HexToByteArray(string _sDataToDecode)
        {
            string sTrimDataToDecode = _sDataToDecode.Replace(" ", string.Empty);

            Console.WriteLine("Dlugosc stringa:" + sTrimDataToDecode.Count());
            byte[] Value = new byte[sTrimDataToDecode.Count() % 2 != 0 ?
                sTrimDataToDecode.Count() / 2 + 1 : sTrimDataToDecode.Count() / 2];
            for (int i = 0; i < Value.Count(); i++)
            {
                try
                {
                    Value[i] = Convert.ToByte(sTrimDataToDecode.Substring(i * 2, 2), 16);
                }
                catch (ArgumentOutOfRangeException outOfRange)
                {
                    Console.WriteLine("Error: " + outOfRange.Message);
                    Value = Value.Take(Value.Count() - 1).ToArray();
                }
            }

            return Value;
        }

        private string HexToBin(string _sDataToDecode)
        {
            string sTrimDataToDecode = _sDataToDecode.Replace(" ", string.Empty);
            string sBinDataToDecode = string.Empty;

            Console.WriteLine("Dlugosc stringa:" + sTrimDataToDecode.Count());
            for (int i = 0; i < sTrimDataToDecode.Count()/2; i++)
            {
                try
                {
                    sBinDataToDecode += Convert.ToUInt32(Convert.ToString(Convert.ToByte(sTrimDataToDecode.Substring(i * 2, 2), 16), 2)).ToString("00000000"); 
                }
                catch (ArgumentOutOfRangeException outOfRange)
                {
                    Console.WriteLine("Error: " + outOfRange.Message);
                }
            }

            return sBinDataToDecode;
        }
    }
}
