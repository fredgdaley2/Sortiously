using System;
using System.Collections.Generic;
using System.IO;
using Sortiously.Framework;

namespace Sortiously
{
    public static class FileParser
    {

        public static void ParseDelimitedFile(string sourceFileName, Action<string[], long> dataReadAction, string delimiter = Constants.Delimiters.Comma, bool trimWhiteSpace = true)
        {
            ParseDelimited(sourceFileName, delimiter, trimWhiteSpace, dataReadAction);
        }

        public static void ParseDelimitedString(StringReader sourceReader, Action<string[], long> dataReadAction, string delimiter = Constants.Delimiters.Comma, bool trimWhiteSpace = true)
        {
            string data = sourceReader.ReadLine();
            dataReadAction(ParseDelimitedLine(data, delimiter, trimWhiteSpace), 1);
        }

        public static string[] ParseDelimitedLine(String source, string delimiter = Constants.Delimiters.Comma, bool trimWhiteSpace = true)
        {
            DelimitedLineParser lineParser = new DelimitedLineParser(Convert.ToChar(delimiter), trimWhiteSpace: trimWhiteSpace);
            DelimitedLineParserResult parserResult = lineParser.Parse(source);
            if (parserResult.Errored)
            {
                throw new ApplicationException(parserResult.ErrorMessage);
            }
            return parserResult.Values;
        }

        public static void ParseFixedWidthFile(string sourceFileName, Action<string[], long> dataReadAction, int[] fieldWidths, bool trimWhiteSpace = true)
        {
            ParseFixedWidth(sourceFileName, fieldWidths, trimWhiteSpace, dataReadAction);
        }


        public static void ParseFixedWidthString(StringReader sourceReader, Action<string[], long> dataReadAction, int[] fieldWidths, bool trimWhiteSpace = true)
        {
            string data = sourceReader.ReadLine();
            FixedWidthLineParser lineParser = new FixedWidthLineParser(fieldWidths, trimWhiteSpace: trimWhiteSpace);
            dataReadAction(lineParser.Parse(data), 1);
        }

        private static IEnumerable<string> LineGenerator(StreamReader sr)
        {
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
        }


        private static void ParseDelimited(string sourceFileName, string delimiter, bool trimWhiteSpace, Action<string[], long> dataReadAction)
        {
            using (StreamReader sr = new StreamReader(sourceFileName))
            {
                long lineNum = 0;
                DelimitedLineParser lineParser = new DelimitedLineParser(Convert.ToChar(delimiter), trimWhiteSpace: trimWhiteSpace);
                foreach (string line in LineGenerator(sr))
                {
                    lineNum++;

                    DelimitedLineParserResult parserResult = lineParser.Parse(line);

                    if (parserResult.Errored)
                    {
                        throw new ApplicationException(parserResult.ErrorMessage);
                    }

                    dataReadAction(parserResult.Values, lineNum);

                }
            }
        }

        private static void ParseFixedWidth(string sourceFileName, int[] fieldWidths, bool trimWhiteSpace, Action<string[], long> dataReadAction)
        {
            using (StreamReader sr = new StreamReader(sourceFileName))
            {
                long lineNum = 0;

                FixedWidthLineParser lineParser = new FixedWidthLineParser(fieldWidths, trimWhiteSpace: trimWhiteSpace);

                foreach (string line in LineGenerator(sr))
                {
                    lineNum++;

                    string[] values = lineParser.Parse(line);

                    dataReadAction(values, lineNum);

                }
            }
        }

    }
}
