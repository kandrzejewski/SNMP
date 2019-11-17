using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SNMP
{
    class RegexExpression
    {
        public string ObjectIdentifier;
        public string ObjectIdentifierRoot;
        public string ObjectType;
        public string DataTypes;
        public string DataSequence;
        public string DataSequenceBody;
        public string Imports;
        public string DeleteComments;

        public string DataTypesSize;
        public string DataTypesMin;
        public string DataTypesMax;

        public RegexOptions Options;

        public RegexExpression()
        {
            Options = RegexOptions.Singleline | RegexOptions.Compiled;

            ObjectIdentifierRoot = @"(?<Name>\S*)\s*OBJECT\sIDENTIFIER\s::=\s{\s" +
                                   @"(?<Parrent1Name>\S*)\s(?<Parrent2Name>\S*)\(" +
                                   @"(?<Parrent2OID>[0-9]*)\)\s(?<Parrent3Name>\S*)\(" +
                                   @"(?<Parrent3OID>[0-9]*)\)\s(?<OID>[0-9]*)\s}";
            ObjectIdentifier = @"(?<Name>\S*)\s*OBJECT\sIDENTIFIER\s::=\s{\s" +
                               @"(?<ParrentName>\S*)\s(?<OID>[1-9]*)\s}\s*";
            ObjectType = @"(?<Name>\S*)\sOBJECT-TYPE\s*SYNTAX\s*(?<SYNTAX>.*?)\s*ACCESS\s*" +
                         @"(?<ACCESS>\S*)\s*STATUS\s*(?<STATUS>\S*)\s*DESCRIPTION\s*\" + "\"" +
                         @"(?<DESCRIPTION>.*?" + "\")" + @".*?::=\s*{\s(?<ParrentName>\S*)" +
                         @"\s(?<OID>[0-9]*)\s}";
            DataTypes = @"(?<TypeName>\S*)\s*::=\s*\[(?<Visibility>\w*)\s(?<TypeID>\d*)" +
                        @"\]\s*(?<EncodingType>\w*)\s*(?<ParrentType>\w*!\s|\w*\s*\w*)" +
                        @"\s(?<Restriction>\(.*?\)|\(.*?\)|)";
            DataTypesSize = @"\(SIZE\s\((?<Value>\d*)\)";
            DataTypesMin = @"\((?<Min>\d*)\.\.\d*\)";
            DataTypesMax = @"\(\d*\.\.(?<Max>\d*)\)";
            DataSequence = @"(?<TypeName>\w*)\s::=\s*SEQUENCE\s\{\s*(?<SequenceBody>.*?)\s*\}";
            DataSequenceBody = @"\s*(?<Name>\w*)\s+(?<Value>\w+\s\w+|\w+\s\(.*?\)|\w+)";
            Imports = @"IMPORTS\s*.*?FROM\s*(?<FileName>\S*)";
            DeleteComments = @"--.*?";


        }
    }
    
}
