using System;
using System.IO;
using System.Linq;
using Sortiously.Framework;

namespace Sortiously
{
    internal static class ArgumentValidation
    {
        public static void Validate<T>(string sourcefilePath, Func<string[], string, T> getKey, string delimiter, string destinationFolder, int maxBatchSize)
        {
            if (getKey == null)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { getKey }), "A GetKey function must be defined.");
            }
            Validate(delimiter);
            Validate(sourcefilePath, destinationFolder);
            Validate(maxBatchSize);
        }

        public static void Validate(string sourcefilePath, Action<string[], string, string[]> setKeys, string delimiter, string destinationFolder)
        {
            Validate(setKeys);
            Validate(delimiter);
            Validate(sourcefilePath, destinationFolder);
        }

        public static void Validate(Action<string[], string, string[]> setKeys)
        {
            if (setKeys == null)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { setKeys }), "A setKeys function must be defined.");
            }
        }

        public static void Validate<T>(string sourcefilePath, Func<string, T> getKey, string destinationFolder, int maxBatchSize)
        {
            if (getKey == null)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { getKey }), "A GetKey function must be defined.");
            }
            Validate(sourcefilePath, destinationFolder);
            Validate(maxBatchSize);
        }

        public static void Validate(string sourcefilePath, Action<string, string[]> setKeys, string destinationFolder)
        {
            if (setKeys == null)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { setKeys }), "A setKeys function must be defined.");
            }
            Validate(sourcefilePath, destinationFolder);
        }


        public static void Validate(string sourcefilePath, string destinationFolder)
        {
            if (string.IsNullOrWhiteSpace(sourcefilePath))
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { sourcefilePath }), "The sourceFilePath cannot be null or empty.");
            }
            if (!File.Exists(sourcefilePath))
            {
                throw new FileNotFoundException("The sourceFilePath , " + sourcefilePath + " , does not exist.");
            }
            if (destinationFolder != null && !Directory.Exists(destinationFolder))
            {
                throw new DirectoryNotFoundException("The destination folder, " + destinationFolder + " , does not exist.");
            }

        }

        public static void Validate(string delimiter)
        {
            if (string.IsNullOrEmpty(delimiter))
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { delimiter }), "The delimiter can not be null or empty.");
            }

        }

        public static void Validate(int maxBatchSize)
        {
            if (maxBatchSize <= 0)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { maxBatchSize }), "The maxBatchSize must be greater than zero.");
            }
            else if (maxBatchSize > int.MaxValue)
            {
                throw new ArgumentNullException(SortHelpers.GetParameterName(new { maxBatchSize }), "The maxBatchSize can not be greater than integer maximum value.");

            }


        }
    }
}
