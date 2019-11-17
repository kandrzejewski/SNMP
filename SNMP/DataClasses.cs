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
        public string Syntax {get; set;}
        public string Access { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string ParrentName { get; set; }
        public int OID { get; set; }

        public void PresentData(int _iWriteIteration)
        {
            WriteData(_iWriteIteration, "Name", Name);
            WriteData(_iWriteIteration, "Syntax", Syntax);
            WriteData(_iWriteIteration, "Access", Access);
            WriteData(_iWriteIteration, "Status", Status);
            WriteData(_iWriteIteration, "Description", Description);
            WriteData(_iWriteIteration, "ParrentName", ParrentName);
            WriteData(_iWriteIteration, "OID", OID);
            WriteData(_iWriteIteration);
        }
    }

    public class SubDataSequence: WriteDataClass
    {
        public string TypeName { get; set; }
        public string SequenceBody { get; set; }

        public void PresentData(int _iWriteIteration)
        {
            WriteData(_iWriteIteration, "TypeName", TypeName);
            WriteData(_iWriteIteration, "SequenceBody", SequenceBody);
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
            WriteData(_iWriteIteration, "TypeName", TypeName);
            WriteData(_iWriteIteration, "Visibility", oOtherData.Visibility);
            WriteData(_iWriteIteration, "TypeID", oOtherData.TypeID);
            WriteData(_iWriteIteration, "EncodingType", oOtherData.EncodingType);
            WriteData(_iWriteIteration, "ParrentType", oOtherData.ParrentType);
            WriteData(_iWriteIteration, "SizeRestriction", oSize.Size);
            WriteData(_iWriteIteration, "RangeRestriction Min", oRange.Min);
            WriteData(_iWriteIteration, "RangeRestriction Max", oRange.Max.ToString());
            
            if (oSequence.lElements.Count > 0)
            {
                WriteData(_iWriteIteration, "Sequence Count", oSequence.lElements.Count);
                foreach (SequenceElement _element in oSequence.lElements)
                {
                    WriteData(_iWriteIteration + 1, _element.ElementName, _element.ElementType);
                    WriteData(_iWriteIteration + 1);
                }
            }
            WriteData(_iWriteIteration);
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
        public string ElementType { get; set; }
    }

    public class WriteDataClass
    {
        public void WriteData(int _iWriteIteration, string _sName, string _sValue )
        {
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine("{0}: {1}", _sName, _sValue);
        }
        public void WriteData(int _iWriteIteration, string _sName, int _sValue)
        {
            WriteTreeNode(_iWriteIteration);
            Console.WriteLine("{0}: {1}", _sName, _sValue);
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
    }
}
