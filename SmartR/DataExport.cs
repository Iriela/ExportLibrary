using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Utf8Json.Resolvers;
using Extension = SmartR.Constants.Extension;

namespace SmartR
{
    /// <summary>
    /// A helper that will help to export data and have the checksum
    /// </summary>
    public static class DataExport
    {

        /// <summary>
        /// Serialize the list of object and Create a json file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contents"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="outputName"></param>
        /// <returns></returns>
        public static string ConvertToJsonFile<T>(List<T> contents, string outputDirectory, string outputName)
        {
            var outputDir = Path.Combine(Environment.CurrentDirectory, outputDirectory);
            var bytesOfNewLine = Encoding.UTF8.GetBytes(Environment.NewLine);
            var jsonFiles = new List<string>();
            Directory.CreateDirectory(outputDir);

            string? jsonFilePath;
            try
            {
                var jsonFileName = $"{outputName}{Extension.Json}";
                jsonFilePath = Path.Combine(outputDir, jsonFileName);
                var jsonBytes = new List<byte>();

                Utf8Json.JsonSerializer.SetDefaultResolver(StandardResolver.AllowPrivateExcludeNullSnakeCase);

                foreach (var content in contents)
                {
                    jsonBytes.AddRange(Utf8Json.JsonSerializer.Serialize(content));
                    jsonBytes.AddRange(bytesOfNewLine);
                }

                File.WriteAllBytes(jsonFilePath, jsonBytes.ToArray());

                jsonFiles.Add(jsonFilePath);
            }
            catch (Exception ex)
            {
                ex.Data.Add("Additional information", $"{DateTime.Now:HH:mm:ss.fff} Json convert failed !");
                throw;
            }

            return jsonFilePath;
        }


        /// <summary>
        /// Compress the file into gzip
        /// </summary>
        /// <param name="fileName"></param>
        public static void CompressFileToGzip(string fileName)
        {
            var gZipFile = $"{fileName}{Extension.Gzip}";

            using var fileStream = new FileStream(gZipFile, FileMode.Create);
            using var gZipStream = new GZipStream(fileStream, CompressionMode.Compress);
            var jsonBytes = File.ReadAllBytes(fileName);

            gZipStream.Write(jsonBytes, 0, jsonBytes.Length);
        }

        /// <summary>
        /// Caclulate MD5 hash of the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CalculateMD5Hash(string fileName)
        {
            var md5 = MD5.Create();

            var hashBytes = md5.ComputeHash(File.ReadAllBytes($"{Path.GetFileName(fileName)}{Extension.Gzip}"));

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("X2"));
            }

            return stringBuilder.ToString();
        }
    }
}