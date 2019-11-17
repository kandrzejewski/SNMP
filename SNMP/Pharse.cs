using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace SNMP
{
    
    class Pharse: Pharser
    {
        RegexExpression oRegexExpression = new RegexExpression();

        public void ReadFromFile(string sFileName)
        {
            try
            {
                var sFile = File.ReadLines(@"../data/mibs/" + sFileName + @".txt");
                string sPreparedFile="";
                foreach (string line in sFile)
                {
                    if(!line.Contains("--"))
                    {
                        sPreparedFile = sPreparedFile + line + "\n";
                    }
                    else
                    {
                        sPreparedFile = sPreparedFile + line.Remove(line.IndexOf('-')) + "\n";
                    }
                }
                //Console.WriteLine(sPreparedFile);
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
            PharseObjectIdentifierRoot(_text);
            PharseObjectIdentifier(_text);
            PharseObjectType(_text);
            PharseImports(_text);
            PharseDataTypes(_text);
            PharseDataSequence(_text);
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
                                    DataHub.lData.Last().Syntax = Regex.Replace(Regex.Replace(_match.Groups[_iGroupNumber].Value, @"\n", ""), @"\s{3,}", " ");
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
                //DataHub.lData.Last().PresentData(1);
                //Console.WriteLine("");
            }
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
                //DataHub.lData.Last().PresentData(1);
                //Console.WriteLine("");
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
                        //Console.WriteLine("Group {0}: {1}", RegexObjectIdentifier.GroupNameFromNumber(_iGroupNumber), _match.Groups[_iGroupNumber].Value);
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
                //DataHub.lData.Last().PresentData(1);
                //Console.WriteLine("");
            }
        }

        private void PharseObjectIdentifierRoot(string _text)
        {
            Regex RegexObjectIdentifierRoot = new Regex(oRegexExpression.ObjectIdentifierRoot, oRegexExpression.Options);

            MatchCollection ObjectIdentifierRootMatches = RegexObjectIdentifierRoot.Matches(_text);
            foreach (Match _match in ObjectIdentifierRootMatches)
            {
                DataHub.lData.Add(new ObjectType());
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(1) == "Name"){
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
                //DataHub.lData.Last().PresentData(1);
                //Console.WriteLine("");
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
                //DataHub.lData.Last().PresentData(1);
                //Console.WriteLine("");
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
                //DataHub.lData.Last().PresentData(1);
                //Console.WriteLine("");
                DataHub.lData.Add(new ObjectType());
                if (RegexObjectIdentifierRoot.GroupNameFromNumber(2) == "Parrent1Name")
                {
                    DataHub.lData.Last().Name = _match.Groups[2].Value;
                }
                //DataHub.lData.Last().PresentData(1);
                //Console.WriteLine("");
            }
        }

        private void PharseDataTypes(string _text)
        {
            Regex RegexDataType = new Regex(oRegexExpression.DataTypes, oRegexExpression.Options);
            Regex RegexRestrictionSize = new Regex(oRegexExpression.DataTypesSize, oRegexExpression.Options);
            Regex RegexRestrictionMin= new Regex(oRegexExpression.DataTypesMin, oRegexExpression.Options);
            Regex RegexRestrictionMax = new Regex(oRegexExpression.DataTypesMax, oRegexExpression.Options);

            MatchCollection DataTypeMatches = RegexDataType.Matches(_text);
            foreach (Match _match in DataTypeMatches)
            {
                DataHub.lDataType.Add(new DataType());
                for (int _iGroupNumber = 1; _iGroupNumber < _match.Groups.Count; _iGroupNumber++)
                {
                    //Console.WriteLine("Group {0}: {1}", RegexObjectIdentifier.GroupNameFromNumber(_iGroupNumber), _match.Groups[_iGroupNumber].Value);
                    switch (RegexDataType.GroupNameFromNumber(_iGroupNumber))
                    {
                        case "TypeName":
                            {
                                DataHub.lDataType.Last().TypeName = _match.Groups[_iGroupNumber].Value;
                                continue;
                            }
                        case "Visibility":
                            {
                                DataHub.lDataType.Last().oOtherData.Visibility = _match.Groups[_iGroupNumber].Value;
                                continue;
                            }
                        case "TypeID":
                            {
                                DataHub.lDataType.Last().oOtherData.TypeID = Int32.Parse(_match.Groups[_iGroupNumber].Value);
                                continue;
                            }
                        case "EncodingType":
                            {
                                DataHub.lDataType.Last().oOtherData.EncodingType = _match.Groups[_iGroupNumber].Value;
                                continue;
                            }
                        case "ParrentType":
                            {
                                DataHub.lDataType.Last().oOtherData.ParrentType =_match.Groups[_iGroupNumber].Value;
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
                DataHub.lDataType.Last().PresentData(1);
            }
        }

        private string ParseRestriction(Regex _RegexRestriction, string _text)
        {
            //Console.WriteLine(_RegexRestriction.Matches(_text)[0].Groups[1].Value);
            if(_RegexRestriction.IsMatch(_text))
                return _RegexRestriction.Matches(_text)[0].Groups[1].Value;
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
                DataHub.lDataType.Add(new DataType()); DataHub.lDataType.Add(new DataType());
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
                                foreach (Match _bodyMatch in DataSequenceMatches)
                                {
                                    DataHub.lDataType.Last().oSequence.lElements.Add(new SequenceElement());
                                    DataHub.lDataType.Last().oSequence.lElements.Last().ElementName = _match.Groups[1].Value;
                                    DataHub.lDataType.Last().oSequence.lElements.Last().ElementName = _match.Groups[2].Value;
                                }
                                continue;
                            }
                    }
                }
                DataHub.lDataType.Last().PresentData(1);
            }
        }
        //private string[] ParseSequenceBody(Regex _RegexBody, string _text)
        //{
            //Console.WriteLine(_RegexRestriction.Matches(_text)[0].Groups[1].Value);
            //if (_RegexBody.IsMatch(_text))
                //string[] _sTemp = { "ddf" };
                //return _RegexBody.Matches(_text)[0].Groups[1].Value;
            //else
            //{
               // return "";
            //}
        //}
    }
}
