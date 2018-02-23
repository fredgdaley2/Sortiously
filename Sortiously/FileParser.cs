using System;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace DarthSortious
{
    public static class FileParser
    {

        public static void ParseDelimitedFile(string sourceFileName, Action<string[], long> dataReadAction, string delimiter = ",")
        {
            TextFieldParser csvReader = new TextFieldParser(sourceFileName);
            csvReader.SetDelimiters(delimiter);
            csvReader.HasFieldsEnclosedInQuotes = delimiter != Delimiters.Tab;
            csvReader.TrimWhiteSpace = true;
            ParseFile(csvReader, dataReadAction);
        }

        public static void ParseDelimitedString(StringReader sourceReader, Action<string[], long> dataReadAction, string delimiter = ",")
        {
            TextFieldParser csvReader = new TextFieldParser(sourceReader);
            csvReader.SetDelimiters(delimiter);
            csvReader.HasFieldsEnclosedInQuotes = delimiter != Delimiters.Tab;
            csvReader.TrimWhiteSpace = true;
            ParseFile(csvReader, dataReadAction);
        }


        public static void ParseFixedWidthFile(string sourceFileName, Action<string[], long> dataReadAction, int[] fieldWidths, bool trimWhiteSpace = true)
        {
            TextFieldParser fwReader = new TextFieldParser(sourceFileName);
            fwReader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.FixedWidth;
            fwReader.FieldWidths = fieldWidths;
            fwReader.TrimWhiteSpace = trimWhiteSpace;
            ParseFile(fwReader, dataReadAction);
        }


        public static void ParseFixedWidthString(StringReader sourceReader, Action<string[], long> dataReadAction, int[] fieldWidths, bool trimWhiteSpace = true)
        {
            TextFieldParser fwReader = new TextFieldParser(sourceReader);
            fwReader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.FixedWidth;
            fwReader.FieldWidths = fieldWidths;
            fwReader.TrimWhiteSpace = trimWhiteSpace;
            ParseFile(fwReader, dataReadAction);
        }


        public static void ParseFile(TextFieldParser txtFldParser, Action<string[], long> dataReadAction)
        {
            using (TextFieldParser parser = txtFldParser)
            {
                try
                {
                    while (!parser.EndOfData)
                    {
                        string[] data = parser.ReadFields();
                        dataReadAction(data, parser.LineNumber - 1);
                    }
                }
                catch (MalformedLineException)
                {
                    throw;
                }

            }
        }

    }
}
