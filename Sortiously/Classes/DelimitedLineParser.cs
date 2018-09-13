using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sortiously.Framework
{
public class DelimitedLineParser
    {

        public char Delimiter { get; set; }
        public int MaxFields { get; set; }
        public bool TrimWhiteSpace { get; set; }
        public bool FieldsEnclosedInQuotes { get; set; }

        public DelimitedLineParser(char delimiter, int maxFields = -1, bool trimWhiteSpace = true, bool fieldsEnclosedInQuotes = true)
        {
            Delimiter = delimiter;
            MaxFields = maxFields;
            TrimWhiteSpace = trimWhiteSpace;
            FieldsEnclosedInQuotes = fieldsEnclosedInQuotes;
        }

        public DelimitedLineParserResult Parse(string lineToParse)
        {
            char[] characters = lineToParse.ToCharArray();
            List<string> returnValueList = new List<string>();
            string tempString = string.Empty;
            bool blockUntilEndQuote = false;
            bool parseErrored = false;
            string errorMsg = string.Empty;
            for (int cIdx = 0; cIdx < characters.Length; cIdx++)
            {
                char character = characters[cIdx];
                if (character == '"' && FieldsEnclosedInQuotes)
                {
                    if (blockUntilEndQuote == false)
                    {
                        blockUntilEndQuote = true;
                    }
                    else if (blockUntilEndQuote == true)
                    {
                        blockUntilEndQuote = false;
                    }
                }

                if (character != Delimiter)
                {
                    tempString = tempString + character;
                }
                else if (character == Delimiter && (blockUntilEndQuote == true))
                {
                    tempString = tempString + character;
                }
                else
                {
                    if (FieldsEnclosedInQuotes)
                    {
                        errorMsg = ParseFieldEnclosedInQuotes(tempString, returnValueList);
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            errorMsg += string.Format(" Field ({0})", returnValueList.Count);
                            parseErrored = true;
                            break;
                        }
                    }
                    else
                    {
                        returnValueList.Add(TrimAllWhiteSpace(tempString));
                    }

                    if (ReachedMaxFields(returnValueList))
                    {
                        break;
                    }
                    tempString = "";
                }

                if (cIdx + 1 == lineToParse.Length)
                {
                    if (FieldsEnclosedInQuotes)
                    {
                        errorMsg = ParseFieldEnclosedInQuotes(tempString, returnValueList);
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            errorMsg += string.Format(" Field ({0})", returnValueList.Count);
                            parseErrored = true;
                            break;
                        }
                    }
                    else
                    {
                        returnValueList.Add(TrimAllWhiteSpace(tempString));
                    }

                    if (ReachedMaxFields(returnValueList))
                    {
                        break;
                    }
                    tempString = "";
                }
            }
            string[] returnValue = returnValueList.ToArray();
            characters = null;
            returnValueList = null;
            tempString = null;
            return new DelimitedLineParserResult() { Values = returnValue, Errored = parseErrored, ErrorMessage = errorMsg };
        }

        public async Task<DelimitedLineParserResult> ParseAsync(string lineToParse)
        {
            return await Task.Run(() => Parse(lineToParse));
        }

        private string ParseFieldEnclosedInQuotes(string tempString, List<string> returnValueList)
        {
            if (!DblQuoteCompliant(tempString))
            {
                return "Malformed Line: " + tempString;
            }
            returnValueList.Add(TrimAllWhiteSpace(RemoveFirstOccurance(RemoveLastOccurance(tempString, "\""), "\"").Replace("\"\"", "\"")));
            return string.Empty;
        }

        private bool ReachedMaxFields(List<string> returnValueList)
        {
            return MaxFields > -1 && returnValueList.Count == MaxFields;
        }

        private bool DblQuoteCompliant(string checkValue)
        {
            if (EnclosedInQuotes(checkValue))
            {
                string firstLastRemoved = RemoveFirstOccurance(RemoveLastOccurance(checkValue, "\""), "\"");
                string escapedDblQuotesRemoved = firstLastRemoved.Replace("\"\"", "");
                return escapedDblQuotesRemoved.IndexOf("\"", StringComparison.Ordinal) == -1;
            }
            return true;
        }

        private bool EnclosedInQuotes(string checkValue)
        {
            if (!string.IsNullOrWhiteSpace(checkValue) && checkValue.Length > 1)
            {
                return Convert.ToString(checkValue[0]) == "\"" && Convert.ToString(checkValue[checkValue.Length - 1]) == "\"";
            }
            return false;

        }

        private string TrimAllWhiteSpace(string source)
        {
            return TrimWhiteSpace ? source.Trim() : source;
        }

        private string RemoveFirstOccurance(string source, string toRemove)
        {
            return string.IsNullOrWhiteSpace(source) ? source
                : Convert.ToString(source[0]) == toRemove ? source.Substring(1) : source;
        }

        private string RemoveLastOccurance(string source, string toRemove)
        {
            return string.IsNullOrWhiteSpace(source) ? source
                : Convert.ToString(source[source.Length - 1]) == toRemove ? source.Substring(0, source.Length - 1) : source;
        }
    }
}
