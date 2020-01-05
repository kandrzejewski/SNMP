using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace SNMP
{

    class Pharse : Pharser
    {
        RegexExpression oRegexExpression = new RegexExpression();

        public void ReadFromFile(string sFileName)
        {
            try
            {
                var sFile = File.ReadLines(@"../data/mibs/" + sFileName + @".txt");
                string sPreparedFile = "";
                foreach (string line in sFile)
                {
                    if (!line.Contains("--"))
                    {
                        sPreparedFile = sPreparedFile + line + "\n";
                    }
                    else
                    {
                        sPreparedFile = sPreparedFile + line.Remove(line.IndexOf('-')) + "\n";
                    }
                }
                Console.WriteLine("Odczytano plik {0}!", sFileName);
                Console.WriteLine("------------------------");
                PharseFile(sPreparedFile);
            }
            catch (IOException e)
            {
                Console.WriteLine("Nie można otworzyć pliku:");
                Console.WriteLine("------------------------");
                Console.WriteLine(e.Message);
            }
        }

        private void OpenNewFile(string _filename)
        {
            Console.Write(@"Wystąpiło wywołanie pliku: ");
            Console.WriteLine(_filename);
            ReadFromFile(_filename);
        }

        private void PharseFile(string _text)
        {
            PharseImports(_text);
            PharseDataTypes(_text);
            PharseDataSequence(_text);
            PharseObjectIdentifierRoot(_text);
            PharseObjectIdentifier(_text);
            PharseObjectType(_text);
        }

        private void PharseImports(string _text)
        {
            Regex RegexImports = new Regex(oRegexExpression.Imports, oRegexExpression.Options);
  
            MatchCollection ImportsMatches = RegexImports.Matches(_text);
            foreach (Match _match in ImportsMatches)
            {
                DataHub.lData.Add(new ObjectType());
                for (int _iGroupNumber = 1; _iGroupNumber < _match.Groups.Count; _iGroupNumber++)
                {
                    if (_match.Groups[_iGroupNumber].Value != "")
                    {
                        //Console.WriteLine("Group {0}: {1}", regEmail.GroupNameFromNumber(_iGroupNumber), _match.Groups[_iGroupNumber].Value);
                        switch (RegexImports.GroupNameFromNumber(_iGroupNumber))
                        {
                            case "FileName":
                                {
                                    OpenNewFile(_match.Groups[_iGroupNumber].Value);
                                    continue;
                                }
                        }
                    }
                }
            }
        }

        private void PharseDataTypes(string _text)
        {
            Regex RegexDataType = new Regex(oRegexExpression.DataTypes, oRegexExpression.Options);
            Regex RegexRestrictionSize = new Regex(oRegexExpression.DataTypesSize, oRegexExpression.Options);
            Regex RegexRestrictionMin = new Regex(oRegexExpression.DataTypesMin, oRegexExpression.Options);
            Regex RegexRestrictionMax = new Regex(oRegexExpression.DataTypesMax, oRegexExpression.Options);

            MatchCollection DataTypeMatches = RegexDataType.Matches(_text);
            foreach (Match _match in DataTypeMatches)
            {
                DataHub.lDataType.Add(new DataType());
                for (int _iGroupNumber = 1; _iGroupNumber < _match.Groups.Count; _iGroupNumber++)
                {
                    //Console.WriteLine("Group {0}: {1}", RegexDataType.GroupNameFromNumber(_iGroupNumber), _match.Groups[_iGroupNumber].Value);
                    if(_match.Groups[_iGroupNumber].Value != "" && _match.Groups[_iGroupNumber].Value != " ")
                    {
                        switch (RegexDataType.GroupNameFromNumber(_iGroupNumber))
                        {
                            case "TypeName":
                                {
                                    DataHub.lDataType.Last().TypeName = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "Class":
                                {
                                    DataHub.lDataType.Last().oOtherData.Class = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "TagNumber":
                                {
                                    DataHub.lDataType.Last().oOtherData.TagNumber = Int32.Parse(_match.Groups[_iGroupNumber].Value);
                                    continue;
                                }
                            case "EncodingType":
                                {
                                    DataHub.lDataType.Last().oOtherData.EncodingType = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "ParrentType":
                                {
                                    DataHub.lDataType.Last().oOtherData.ParrentType = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "Restriction":
                                {
                                    if (ParseRestriction(RegexRestrictionSize, _match.Groups[_iGroupNumber].Value) != "")
                                        DataHub.lDataType.Last().oSize.Size =
                                          Int32.Parse(ParseRestriction(RegexRestrictionSize, _match.Groups[_iGroupNumber].Value));
                                    if (ParseRestriction(RegexRestrictionMin, _match.Groups[_iGroupNumber].Value) != "")
                                        DataHub.lDataType.Last().oRange.Min = 
                                          Int32.Parse(ParseRestriction(RegexRestrictionMin, _match.Groups[_iGroupNumber].Value));
                                    if (ParseRestriction(RegexRestrictionMax, _match.Groups[_iGroupNumber].Value) != "")
                                        DataHub.lDataType.Last().oRange.Max =
                                          Int64.Parse(ParseRestriction(RegexRestrictionMax, _match.Groups[_iGroupNumber].Value));
                                    continue;
                                }
                        }
                    } 
                }
            }
        }

        private string ParseRestriction(Regex _RegexRestriction, string _text)
        {
            
            if (_RegexRestriction.IsMatch(_text))
            {
                return _RegexRestriction.Matches(_text)[0].Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        private void PharseDataSequence(string _text)
        {
            Regex RegexDataSequence = new Regex(oRegexExpression.DataSequence, oRegexExpression.Options);
            Regex RegexSequenceBody = new Regex(oRegexExpression.DataSequenceBody, oRegexExpression.Options);

            MatchCollection DataSequenceMatches = RegexDataSequence.Matches(_text);
            foreach (Match _match in DataSequenceMatches)
            {
                DataHub.lDataType.Add(new DataType());
                DataHub.lDataType.Last().oOtherData.ParrentType = "SEQUENCE";
                for (int _iGroupNumber = 1; _iGroupNumber < _match.Groups.Count; _iGroupNumber++)
                {
                    //Console.WriteLine("Group {0}: {1}", RegexObjectIdentifier.GroupNameFromNumber(_iGroupNumber), _match.Groups[_iGroupNumber].Value);
                    switch (RegexDataSequence.GroupNameFromNumber(_iGroupNumber))
                    {
                        case "TypeName":
                            {
                                DataHub.lDataType.Last().TypeName = _match.Groups[_iGroupNumber].Value;
                                continue;
                            }
                        case "SequenceBody":
                            {
                                string sTempMatch = Regex.Replace(Regex.Replace(_match.Groups[_iGroupNumber].Value, @"\n", ""), @"\s{3,}", " ");
                                MatchCollection BodyMatches = RegexSequenceBody.Matches(sTempMatch);
                                //Console.WriteLine(sTempMatch);
                                foreach (Match _bodyMatch in BodyMatches)
                                {
                                    DataHub.lDataType.Last().oSequence.lElements.Add(new SequenceElement());
                                    DataHub.lDataType.Last().oSequence.lElements.Last().ElementName = _bodyMatch.Groups[1].Value;
                                    DataHub.lDataType.Last().oSequence.lElements.Last().ElementType = 
                                        DataHub.lDataType[PharseSyntaxDataType(_bodyMatch.Groups[2].Value)];
                                }
                                continue;
                            }
                    }
                }
            }
        }

        private void PharseObjectIdentifier(string _text)
        {
            Regex RegexObjectIdentifier = new Regex(oRegexExpression.ObjectIdentifier, oRegexExpression.Options);

            MatchCollection ObjectIdentifierMatches = RegexObjectIdentifier.Matches(_text);
            foreach (Match _match in ObjectIdentifierMatches)
            {
                DataHub.lData.Add(new ObjectType());
                for (int _iGroupNumber = 1; _iGroupNumber < _match.Groups.Count; _iGroupNumber++)
                {
                    if (_match.Groups[_iGroupNumber].Value != "")
                    {
                        switch (RegexObjectIdentifier.GroupNameFromNumber(_iGroupNumber))
                        {
                            case "Name":
                                {
                                    DataHub.lData.Last().Name = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "ParrentName":
                                {
                                    DataHub.lData.Last().ParrentName = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "OID":
                                {
                                    DataHub.lData.Last().OID = Int32.Parse(_match.Groups[_iGroupNumber].Value);
                                    continue;
                                }
                        }
                    }
                }
            }
        }

        private void PharseObjectIdentifierRoot(string _text)
        {
            Regex RegexObjectIdentifierRoot = new Regex(oRegexExpression.ObjectIdentifierRoot, oRegexExpression.Options);

            MatchCollection ObjectIdentifierRootMatches = RegexObjectIdentifierRoot.Matches(_text);
            foreach (Match _match in ObjectIdentifierRootMatches)
            {
                DataHub.lData.Add(new ObjectType());
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(1) == "Name")
                {
                    DataHub.lData.Last().Name = _match.Groups[1].Value;
                }
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(5) == "Parrent3Name")
                {
                    DataHub.lData.Last().ParrentName = _match.Groups[5].Value;
                }
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(7) == "OID")
                {
                    DataHub.lData.Last().OID = Int32.Parse(_match.Groups[7].Value);
                }
                DataHub.lData.Add(new ObjectType());
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(5) == "Parrent3Name")
                {
                    DataHub.lData.Last().Name = _match.Groups[5].Value;
                }
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(3) == "Parrent2Name")
                {
                    DataHub.lData.Last().ParrentName = _match.Groups[3].Value;
                }
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(6) == "Parrent3OID")
                {
                    DataHub.lData.Last().OID = Int32.Parse(_match.Groups[6].Value);
                }
                DataHub.lData.Add(new ObjectType());
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(3) == "Parrent2Name")
                {
                    DataHub.lData.Last().Name = _match.Groups[3].Value;
                }
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(2) == "Parrent1Name")
                {
                    DataHub.lData.Last().ParrentName = _match.Groups[2].Value;
                }
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(4) == "Parrent2OID")
                {
                    DataHub.lData.Last().OID = Int32.Parse(_match.Groups[4].Value);
                }
                DataHub.lData.Add(new ObjectType());
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(2) == "Parrent1Name")
                {
                    DataHub.lData.Last().Name = _match.Groups[2].Value;
                    DataHub.lData.Last().OID = 1;
                }
            }
        }

        private void PharseObjectType(string _text)
        {
            Regex RegexObjectType = new Regex(oRegexExpression.ObjectType, oRegexExpression.Options);
            MatchCollection ObjectTypeMatches = RegexObjectType.Matches(_text);

            foreach (Match _match in ObjectTypeMatches)
            {
                DataHub.lData.Add(new ObjectType());
                for (int _iGroupNumber = 1; _iGroupNumber < _match.Groups.Count; _iGroupNumber++)
                {
                    if (_match.Groups[_iGroupNumber].Value != "")
                    {
                        //Console.WriteLine("Group {0}: {1}", regEmail.GroupNameFromNumber(_iGroupNumber), _match.Groups[_iGroupNumber].Value);
                        switch (RegexObjectType.GroupNameFromNumber(_iGroupNumber))
                        {
                            case "Name":
                                {
                                    DataHub.lData.Last().Name = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "SYNTAX":
                                {
                                    string sTempSyntax = Regex.Replace(Regex.Replace(_match.Groups[_iGroupNumber].Value, @"\n", ""), @"\s{3,}", " ");
                                    DataHub.lData.Last().Syntax =
                                        DataHub.lDataType[PharseSyntaxDataType(sTempSyntax)];
                                    continue;
                                }
                            case "ACCESS":
                                {
                                    DataHub.lData.Last().Access = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "STATUS":
                                {
                                    DataHub.lData.Last().Status = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "DESCRIPTION":
                                {
                                    DataHub.lData.Last().Description = Regex.Replace(Regex.Replace(_match.Groups[_iGroupNumber].Value, @"\n", ""), @"\s{3,}", "");
                                    continue;
                                }
                            case "ParrentName":
                                {
                                    DataHub.lData.Last().ParrentName = _match.Groups[_iGroupNumber].Value;
                                    continue;
                                }
                            case "OID":
                                {
                                    DataHub.lData.Last().OID = Int32.Parse(_match.Groups[_iGroupNumber].Value);
                                    continue;
                                }
                        }
                    }
                }
            }
        }

        private int PharseSyntaxDataType(string _text)
        {
            DataType _DataType = new DataType();
            Regex RegexSyntaxOneWord = new Regex(oRegexExpression.SyntaxOneWord, oRegexExpression.Options);
            Regex RegexSyntaxRestriction = new Regex(oRegexExpression.SyntaxRestriction, oRegexExpression.Options);
            Regex RegexSyntaxTwoWord = new Regex(oRegexExpression.SyntaxTwoWord, oRegexExpression.Options);
            Regex RegexSyntaxSequence = new Regex(oRegexExpression.SyntaxSequence, oRegexExpression.Options);
            Regex RegexSyntaxList = new Regex(oRegexExpression.SyntaxList, oRegexExpression.Options);

            int Index = -1;

            if (RegexSyntaxList.IsMatch(_text))
            {
                MatchCollection Matches = RegexSyntaxList.Matches(_text);
                _DataType.oOtherData.ParrentType = Matches[0].Groups[1].Value;    
                PharseSyntaxDataTypeBody(Matches[0].Groups[2].Value, _DataType);
                Index = SearchDataType(_DataType, "List");
                return Index;
            }
            else if (RegexSyntaxSequence.IsMatch(_text))
            {
                MatchCollection Matches = RegexSyntaxSequence.Matches(_text);            
                _DataType.TypeName = Matches[0].Groups[1].Value;
                Index = SearchDataType(_DataType, "OnlyName");
                return Index;
            }
            else if (RegexSyntaxTwoWord.IsMatch(_text))
            {
                MatchCollection Matches = RegexSyntaxTwoWord.Matches(_text);
                _DataType.TypeName = Matches[0].Groups[1].Value;
                Index = SearchDataType(_DataType, "OnlyName");
                return Index;
            }
            else if (RegexSyntaxRestriction.IsMatch(_text))
            {
                MatchCollection Matches = RegexSyntaxRestriction.Matches(_text);
                _DataType.oOtherData.ParrentType = Matches[0].Groups[3].Value;
                _DataType.oRange.Min = Int32.Parse(Matches[0].Groups[4].Value);
                _DataType.oRange.Max = Int64.Parse(Matches[0].Groups[5].Value);
                Index = SearchDataType(_DataType, "Restrictions");
                return Index;
            }
            else if (RegexSyntaxOneWord.IsMatch(_text))
            {
                MatchCollection Matches = RegexSyntaxOneWord.Matches(_text);
                _DataType.TypeName = Matches[0].Groups[1].Value;
                Index = SearchDataType(_DataType, "OnlyName");
                return Index;
            }
            return Index;
        }

        private void PharseSyntaxDataTypeBody(string _text, DataType _DataType)
        {
            Regex RegexSyntaxListBody = new Regex(oRegexExpression.SyntaxListBody, oRegexExpression.Options);
            MatchCollection Matches = RegexSyntaxListBody.Matches(_text);

            int iMinVal = Int32.Parse(Matches[0].Groups[2].Value);
            long iMaxVal = Int64.Parse(Matches[0].Groups[2].Value);

            foreach (Match _match in Matches)
            {
                _DataType.oSequence.lElements.Add(new SequenceElement());
                if (_match.Groups[3].Value != "")
                {
                    _DataType.oSequence.lElements.Last().ElementName = _match.Groups[3].Value;
                }
                else
                {
                    _DataType.oSequence.lElements.Last().ElementName = _match.Groups[1].Value;
                    if (iMinVal > Int32.Parse(_match.Groups[2].Value))
                    {
                        iMinVal = Int32.Parse(_match.Groups[2].Value);
                    }
                    if (iMaxVal < Int64.Parse(_match.Groups[2].Value))
                    {
                        iMaxVal = Int64.Parse(_match.Groups[2].Value);
                    }
                }
            }
            _DataType.oRange.Min = iMinVal;
            _DataType.oRange.Max = iMaxVal; 
        }

        private int SearchDataType(DataType _DataType, string _NameOfSyntax)
        {
            bool ExistOnList = false;

            foreach (DataType _ListDataType in DataHub.lDataType)
            {
                if (_NameOfSyntax == "List")
                {
                    if (_DataType.oOtherData.ParrentType == _ListDataType.oOtherData.ParrentType
                        && _DataType.oRange.Min == _ListDataType.oRange.Min
                        && _DataType.oRange.Max == _ListDataType.oRange.Max
                        && CompareSequence(_DataType.oSequence, _ListDataType.oSequence))
                    {
                        ExistOnList = true;
                        return DataHub.lDataType.IndexOf(_ListDataType);
                    }
                }
                if (_NameOfSyntax == "OnlyName")
                {
                    if (_DataType.TypeName == _ListDataType.TypeName)
                    {
                        ExistOnList = true;
                        return DataHub.lDataType.IndexOf(_ListDataType);
                    }
                }
                if (_NameOfSyntax == "Restrictions")
                {
                    if (_DataType.oOtherData.ParrentType == _ListDataType.oOtherData.ParrentType
                        && _DataType.oRange.Min == _ListDataType.oRange.Min
                        && _DataType.oRange.Max == _ListDataType.oRange.Max)
                    {
                        ExistOnList = true;
                        return DataHub.lDataType.IndexOf(_ListDataType);
                    }
                }
            }
            if (!ExistOnList)
            {
                DataHub.lDataType.Add(_DataType);
                return DataHub.lDataType.IndexOf(_DataType);
            }
            else
            {
                return 0;
            }  
        }

        private bool CompareSequence(Sequence _oSequence1, Sequence _oSequence2)
        {

            for(int i=0; i < _oSequence1.lElements.Count(); i++)
            {
                    if (_oSequence1.lElements[i].ElementName != _oSequence2.lElements[i].ElementName)
                        return false;
                    if (_oSequence1.lElements[i].ElementType != _oSequence2.lElements[i].ElementType)
                        return false;
            }
            return true;
        }
    }
}
