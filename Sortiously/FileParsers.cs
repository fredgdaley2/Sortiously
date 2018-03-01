using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using Sortiously.Framework;
namespace Sortiously
{
    public static class FileParser
    {

        public static void ParseDelimitedFile(string sourceFileName, Action<string[], long> dataReadAction, string delimiter = Constants.Delimiters.Comma, bool trimWhiteSpace = true)
        {
            TextFieldParser csvReader = new TextFieldParser(sourceFileName)
            {
                Delimiters = new string[] { delimiter },
                HasFieldsEnclosedInQuotes = delimiter != Constants.Delimiters.Tab,
                TrimWhiteSpace = trimWhiteSpace
            };
            ParseFile(csvReader, dataReadAction);
        }

        public static void ParseDelimitedString(StringReader sourceReader, Action<string[], long> dataReadAction, string delimiter = Constants.Delimiters.Comma, bool trimWhiteSpace = true)
        {
            TextFieldParser csvReader = new TextFieldParser(sourceReader)
            {
                Delimiters = new string[] { delimiter },
                HasFieldsEnclosedInQuotes = delimiter != Constants.Delimiters.Tab,
                TrimWhiteSpace = trimWhiteSpace
            };
            ParseFile(csvReader, dataReadAction);
        }


        public static void ParseFixedWidthFile(string sourceFileName, Action<string[], long> dataReadAction, int[] fieldWidths, bool trimWhiteSpace = true)
        {
            TextFieldParser fwReader = new TextFieldParser(sourceFileName)
            {
                TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.FixedWidth,
                FieldWidths = fieldWidths,
                TrimWhiteSpace = trimWhiteSpace
            };
            ParseFile(fwReader, dataReadAction);
        }


        public static void ParseFixedWidthString(StringReader sourceReader, Action<string[], long> dataReadAction, int[] fieldWidths, bool trimWhiteSpace = true)
        {
            TextFieldParser fwReader = new TextFieldParser(sourceReader)
            {
                TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.FixedWidth,
                FieldWidths = fieldWidths,
                TrimWhiteSpace = trimWhiteSpace
            };
            ParseFile(fwReader, dataReadAction);
        }


        public static void ParseFile(TextFieldParser txtFldParser, Action<string[], long> dataReadAction)
        {
            using (TextFieldParser parser = txtFldParser)
            {
                int lineNum = 1;
                while (!parser.EndOfData)
                {
                    string[] data = parser.ReadFields();
                    dataReadAction(data, lineNum);
                    lineNum++;
                }

            }
        }

    }
}
