using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SNMP
{
    public class ObjectType: WriteDataClass
    {
        public string Name { get; set; }
        public DataType Syntax {get; set;}
        public string Access { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string ParrentName { get; set; }
        public int OID { get; set; }

        public ObjectType()
        {
            Syntax = new DataType();
        }

        public void PresentData(int _iWriteIteration)
        {
            WriteData(_iWriteIteration, "Name", Name);
            WriteData(_iWriteIteration, "Syntax", "");
            Syntax.PresentData(_iWriteIteration + 1);
            if (!EmptyCheck(Access)) WriteData(_iWriteIteration, "Access", Access);
            if (!EmptyCheck(Status)) WriteData(_iWriteIteration, "Status", Status);
            if (!EmptyCheck(Description)) WriteData(_iWriteIteration, "Description", Description);
            if (!EmptyCheck(ParrentName)) WriteData(_iWriteIteration, "ParrentName", ParrentName);
            if (!EmptyCheck(OID)) WriteData(_iWriteIteration, "OID", OID);
            WriteData(_iWriteIteration);
        }
    }

    public class DataType: WriteDataClass
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
            if (!EmptyCheck(TypeName)) WriteData(_iWriteIteration, "TypeName", TypeName);
            if (!EmptyCheck(oOtherData.Visibility)) WriteData(_iWriteIteration, "Visibility", oOtherData.Visibility);
            if (!EmptyCheck(oOtherData.TypeID)) WriteData(_iWriteIteration, "TypeID", oOtherData.TypeID);
            if (!EmptyCheck(oOtherData.EncodingType)) WriteData(_iWriteIteration, "EncodingType", oOtherData.EncodingType);
            if (!EmptyCheck(oOtherData.ParrentType)) WriteData(_iWriteIteration, "ParrentType", oOtherData.ParrentType);
            if (!EmptyCheck(oSize.Size)) WriteData(_iWriteIteration, "SizeRestriction", oSize.Size);
            if (!EmptyCheck(oRange.Max))
            {
                WriteData(_iWriteIteration, "RangeRestriction Min", oRange.Min);
                WriteData(_iWriteIteration, "RangeRestriction Max", oRange.Max.ToString());
            }     
            if (!EmptyCheck(oSequence.lElements.Count)) WriteData(_iWriteIteration, "Sequence Count", oSequence.lElements.Count);
            if (oSequence.lElements.Count > 0)
            {
                foreach (SequenceElement _element in oSequence.lElements)
                {
                    WriteData(_iWriteIteration + 1, _element.ElementName, _element.ElementType);
                }
            }
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

    public class WriteDataClass
    {
        public void WriteData(int _iWriteIteration, string _sName, string _sValue)
        {
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine("{0}: {1}", _sName, _sValue);
        }
        public void WriteData(int _iWriteIteration, string _sName, int _sValue)
        {
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine("{0}: {1}", _sName, _sValue);
        }
        public void WriteData(int _iWriteIteration, string _sName, DataType _sObject)
        {
            WriteTreeNode(_iWriteIteration);
            if(_sObject != null)
            {
                Console.WriteLine("{0}:", _sName);
                _sObject.PresentData(_iWriteIteration + 1);
            }
            else
                Console.WriteLine(_sName);
        }
        public void WriteData(int _iWriteIteration)
        {
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine("------");
        }

        private void WriteTreeNode(int _iWriteIteration)
        {
            for (int i = 0; i < _iWriteIteration; i++)
            {
                Console.Write("     |");
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
