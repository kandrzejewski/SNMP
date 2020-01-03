using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SNMP
{
    public class ObjectType : WriteDataClass
    {
        public string Name { get; set; }
        public DataType Syntax { get; set; }
        public string Access { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string ParrentName { get; set; }
        public int OID { get; set; }

        public ObjectType()
        {
            Syntax = new DataType();
            Syntax = null;
        }

        public void PresentData(int _iWriteIteration)
        {
            WriteData(_iWriteIteration, ConsoleColor.Cyan, ConsoleColor.DarkMagenta, "Name", Name, false);
            WriteData(_iWriteIteration, ConsoleColor.Cyan, ConsoleColor.DarkMagenta, "Syntax", "", false);
            if (Syntax != null)
                Syntax.PresentData(_iWriteIteration + 1);
            if (!EmptyCheck(Access)) WriteData(_iWriteIteration, ConsoleColor.Cyan,
                ConsoleColor.DarkMagenta, "Access", Access, false);
            if (!EmptyCheck(Status)) WriteData(_iWriteIteration, ConsoleColor.Cyan,
                ConsoleColor.DarkMagenta, "Status", Status, false);
            if (!EmptyCheck(Description))
                if (Description.Count() < 55)
                    WriteData(_iWriteIteration, ConsoleColor.Cyan,
                    ConsoleColor.DarkMagenta, "Description", Description, false);
                else
                    WriteData(_iWriteIteration, ConsoleColor.Cyan,
                    ConsoleColor.DarkMagenta, "Description", Description.Substring(0, 55) + "...", false);
            if (!EmptyCheck(ParrentName)) WriteData(_iWriteIteration, ConsoleColor.Cyan,
                ConsoleColor.DarkMagenta, "ParrentName", ParrentName, false);
            if (!EmptyCheck(OID)) WriteData(_iWriteIteration, ConsoleColor.Cyan,
                ConsoleColor.Yellow, "OID", OID, false);
            WriteData(_iWriteIteration, true);
        }
    }

    public class DataType : WriteDataClass
    {
        public string TypeName { get; set; }
        public SizeRestriction oSize;
        public RangeRestriction oRange;
        public OtherData oOtherData;
        public Sequence oSequence;

        public DataType()
        {
            oSize = new SizeRestriction();
            oRange = new RangeRestriction();
            oOtherData = new OtherData();
            oSequence = new Sequence();
        }

        public void PresentData(int _iWriteIteration)
        {
            if (!EmptyCheck(TypeName))
                WriteData(_iWriteIteration, ConsoleColor.DarkGreen,
                ConsoleColor.Magenta, "TypeName", TypeName, false);
            if (!EmptyCheck(oOtherData.Visibility))
                WriteData(_iWriteIteration, ConsoleColor.DarkGreen,
                ConsoleColor.Magenta, "Visibility", oOtherData.Visibility, false);
            if (!EmptyCheck(oOtherData.TypeID))
                WriteData(_iWriteIteration, ConsoleColor.DarkGreen,
                ConsoleColor.Magenta, "TypeID", oOtherData.TypeID, false);
            if (!EmptyCheck(oOtherData.EncodingType))
                WriteData(_iWriteIteration, ConsoleColor.DarkGreen,
                ConsoleColor.Magenta, "EncodingType", oOtherData.EncodingType, false);
            if (!EmptyCheck(oOtherData.ParrentType))
                WriteData(_iWriteIteration, ConsoleColor.DarkGreen,
                ConsoleColor.Magenta, "ParrentType", oOtherData.ParrentType, false);
            if (!EmptyCheck(oSize.Size))
                WriteData(_iWriteIteration, ConsoleColor.DarkGreen,
                ConsoleColor.Yellow, "SizeRestriction", oSize.Size, true);
            if (!EmptyCheck(oRange.Max))
            {
                WriteData(_iWriteIteration, ConsoleColor.DarkGreen,
                    ConsoleColor.Yellow, "RangeRestriction Min", oRange.Min, false);
                WriteData(_iWriteIteration, ConsoleColor.DarkGreen,
                    ConsoleColor.Yellow, "RangeRestriction Max", oRange.Max.ToString(), false);
            }
            if (!EmptyCheck(oSequence.lElements.Count))
                WriteData(_iWriteIteration, ConsoleColor.DarkGreen,
                ConsoleColor.Yellow, "Sequence Count", oSequence.lElements.Count, true);
            if (oSequence.lElements.Count > 0)
            {
                foreach (SequenceElement _element in oSequence.lElements)
                {
                    if (_element != oSequence.lElements.Last())
                        WriteData(_iWriteIteration + 1, ConsoleColor.DarkGreen, _element.ElementName, _element.ElementType, false);
                    else
                        WriteData(_iWriteIteration + 1, ConsoleColor.DarkGreen, _element.ElementName, _element.ElementType, true);
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    public class SizeRestriction
    {
        public int Size { get; set; }
    }

    public class RangeRestriction
    {
        public int Min { get; set; }
        public long Max { get; set; }
    }

    public class OtherData
    {
        public string Visibility { get; set; }
        public int TypeID { get; set; }
        public string EncodingType { get; set; }
        public string ParrentType { get; set; }
    }

    public class Sequence
    {
        public List<SequenceElement> lElements = new List<SequenceElement>();
    }

    public class SequenceElement
    {
        public string ElementName { get; set; }
        public DataType ElementType { get; set; }
    }

    public class TreeLeaf
    {
        public List<TreeLeaf> Childrens;
        public ObjectType oObjectType;
        public TreeLeaf()
        {
            oObjectType = new ObjectType();
            Childrens = new List<TreeLeaf>();
        }
    }

    public class EncoderData
    {
        public DataType _oDataType;
        public bool bCanEncode;
        public List<string> ValueToEncode;
        public string sErrorDescription = string.Empty;

        public EncoderData()
        {
            _oDataType = new DataType();
            bCanEncode = false;
            ValueToEncode = new List<string>();
        }

        public void PresentData()
        {
            Console.WriteLine("Data Type to encode:");
            _oDataType.PresentData(1);
            if (sErrorDescription == string.Empty)
            {
                Console.WriteLine("\nData that you entered:");
                foreach (string _value in ValueToEncode)
                    Console.WriteLine(_value);
            }
            Console.Write("Can the data be encoded? ");
            if (bCanEncode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Yes");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            if (sErrorDescription != string.Empty)
            {
                Console.Write("\nError description: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(sErrorDescription);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }

    public class WriteDataClass
    {
        public static int start;
        public void WriteData(int _iWriteIteration, ConsoleColor _color,
            ConsoleColor _color2, string _sName, string _sValue, bool _last)
        {
            WriteTreeNode(_iWriteIteration, _last);
            Console.ForegroundColor = _color;
            Console.Write("{0}:", _sName);
            Console.ForegroundColor = _color2;
            Console.WriteLine(" {0}", _sValue);
        }
        public void WriteData(int _iWriteIteration, ConsoleColor _color,
            ConsoleColor _color2, string _sName, int _sValue, bool _last)
        {
            WriteTreeNode(_iWriteIteration, _last);
            Console.ForegroundColor = _color;
            Console.Write("{0}:", _sName);
            Console.ForegroundColor = _color2;
            Console.WriteLine(" {0}", _sValue);
        }
        public void WriteData(int _iWriteIteration, ConsoleColor _color,
            string _sName, DataType _sObject, bool _last)
        {
            WriteTreeNode(_iWriteIteration, _last);
            if (_sObject != null)
            {
                Console.ForegroundColor = _color;
                Console.WriteLine("{0}:", _sName);
                _sObject.PresentData(_iWriteIteration + 1);
            }
            else
            {
                Console.ForegroundColor = _color;
                Console.WriteLine(_sName);
            }
        }
        public void WriteData(int _iWriteIteration, bool _last)
        {
            WriteTreeNode(_iWriteIteration, _last);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("─────────────");
        }

        private void WriteTreeNode(int _iWriteIteration, bool _last)
        {
            for (int i = 0; i < _iWriteIteration; i++)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                if (i == _iWriteIteration-1)
                    if(_last)
                        Console.Write(" └─");
                    else
                        Console.Write(" ├─");
                else
                    Console.Write(" │   ");

            }
            
        }

        public bool EmptyCheck(string _Element)
        {
            if(_Element == null)
                return true;
            else
                return false;
        }

        public bool EmptyCheck(int _Element)
        {
            if (_Element == 0)
                return true;
            else
                return false;
        }

        public bool EmptyCheck(long _Element)
        {
            if (_Element == 0)
                return true;
            else
                return false;
        }
    }
}
