using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sortiously.Framework
{
    public class FixedWidthLineParser
    {
        public int[] Widths { get; set; }
        public int MaxFields { get; set; }
        public bool TrimWhiteSpace { get; set; }

        public FixedWidthLineParser(int[] widths, bool trimWhiteSpace = true, int maxFields = -1)
        {
            Widths = widths;
            TrimWhiteSpace = trimWhiteSpace;
            MaxFields = maxFields;
        }

        public string[] Parse(string lineToParse)
        {
            char[] characters = lineToParse.ToCharArray();
            List<string> returnValueList = new List<string>();
            int startPosition = 0;
            string value = string.Empty;

            for (int widthIdx = 0; widthIdx < Widths.Length; widthIdx++)
            {
                value = string.Empty;

                for (int positionIdx = 0; positionIdx < Widths[widthIdx]; positionIdx++)
                {
                    value += characters[startPosition + positionIdx];
                }

                returnValueList.Add(TrimWhiteSpace ? value.Trim() : value);

                if (MaxFields > -1 && returnValueList.Count == MaxFields)
                {
                    break;
                }
                startPosition += Widths[widthIdx];

            }
            string[] returnValue = returnValueList.ToArray();
            characters = null;
            returnValueList = null;
            return returnValue;
        }

        public async Task<string[]> ParseAsync(string lineToParse)
        {
            return await Task.Run(() => Parse(lineToParse));
        }
    }
}
