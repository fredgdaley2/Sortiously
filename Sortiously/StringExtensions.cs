using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Sortiously.Framework;

namespace Sortiously.StringExtns
{
    public static class StringExtensions
    {

        /// <summary>
        /// Compresses a string and returns a deflate compressed, Base64 encoded string.
        /// </summary>
        /// <param name="uncompressedString">String to compress</param>
        internal static string Compress(this string uncompressedString)
        {
            var compressedStream = new MemoryStream();
            var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString));

            using (var compressorStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
            {
                uncompressedStream.CopyTo(compressorStream);
            }

            return Convert.ToBase64String(compressedStream.ToArray());
        }

        /// <summary>
        /// Decompresses a deflate compressed, Base64 encoded string and returns an uncompressed string.
        /// </summary>
        /// <param name="compressedString">String to decompress.</param>
        internal static string Decompress(this string compressedString)
        {
            var decompressedStream = new MemoryStream();
            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

            using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                decompressorStream.CopyTo(decompressedStream);
            }

            return Encoding.UTF8.GetString(decompressedStream.ToArray());
        }


        internal static string SerializeToDelimited(this string[] fields, string delimiter = Constants.Delimiters.Comma)
        {
            StringBuilder sb = new StringBuilder();
            int numFieldsAdded = 0;
            int numDelimetersAdded = 0;
            for (int j = 0; j <= fields.Length - 1; j++)
            {
                string value = fields[j];

                //Check if the value contans a comma and place it in quotes if so
                if (delimiter == Constants.Delimiters.Comma && value.Contains(Constants.Delimiters.Comma))
                {
                    //value = string.Concat("\"", value, "\"");
                    value = value.Replace("\"", "\"\"");
                    value = string.Concat("\"", value, "\"");
                }

                //Replace any \r or \n special characters from a new line with a space
                if (value.Contains("\r"))
                {
                    value = value.Replace("\r", " ");
                }
                if (value.Contains("\n"))
                {
                    value = value.Replace("\n", " ");
                }

                sb.Append(value);
                numFieldsAdded++;

                if (j < fields.Length - 1)
                {
                    sb.Append(delimiter.ToCharArray());
                    numDelimetersAdded++;
                }
            }

            string result = sb.ToString();
            if ((numDelimetersAdded > numFieldsAdded) && result.EndsWith(delimiter, StringComparison.Ordinal))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        internal static string SerializeToFixedWidth(this string[] fields, int[] fixedFieldWidths, bool leftAlign = true, bool autoTruncate = true)
        {
            StringBuilder sb = new StringBuilder();

            for (int j = 0; j <= fields.Length - 1; j++)
            {
                    string value = fields[j];
                    if (autoTruncate)
                    {
                        value = value.TruncateFixed(fixedFieldWidths[j]);
                    }
                    if (leftAlign)
                    {
                        value = value.LeftAlign(fixedFieldWidths[j]);
                    }
                    else
                    {
                        value = value.RightAlign(fixedFieldWidths[j]);
                    }
                    sb.Append(value);
            }

            return sb.ToString();
        }


        public static string LeftAlign(this string value, int totWidth)
        {
            return value.PadRight(totWidth);
        }
        public static string RightAlign(this string value, int totWidth)
        {
            return value.PadLeft(totWidth);
        }

        public static string TruncateFixed(this string value, int totWidth)
        {
            if (value.Length > totWidth)
            {
                value = value.Substring(0, totWidth);
            }
            return value;
        }

        public static string PadKeyWithZero(this string value, int keyLength)
        {
            return value.Trim().PadLeft(keyLength, '0');
        }

        public static string PadKeyWithA(this string value, int keyLength)
        {
            return value.Trim().PadRight(keyLength, 'A');
        }


    }
}
