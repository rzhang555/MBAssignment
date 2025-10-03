using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Reflection.Metadata;
using MBAssignment.Configurations;
using System.Reflection;

namespace MBAssignment.Services
{
    public static class FileService
    {
        public static void CheckDirectories(AppSettings settings)
        {
            try { 
                if (!Directory.Exists(settings.Input_dir))
                    Directory.CreateDirectory(settings.Input_dir);

                if (!Directory.Exists(settings.Output_dir))
                    Directory.CreateDirectory(settings.Output_dir);

                if (!Directory.Exists(settings.Failed_dir))
                    Directory.CreateDirectory(settings.Failed_dir);

                if (!Directory.Exists(settings.Archive_dir))
                    Directory.CreateDirectory(settings.Archive_dir);

                if (!Directory.Exists(settings.Logfile_dir))
                    Directory.CreateDirectory(settings.Logfile_dir);

            } catch (Exception ex) {
                LoggingService.WriteErr(ex.Message);
            }
        }
        public static bool CopyFile(string sourceFilePath, string targetFilePath)
        {
            try
            {

                if (!File.Exists(targetFilePath))
                {
                    File.Copy(sourceFilePath, targetFilePath);
                }
                else
                    //log failed and reason
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                LoggingService.WriteErr(ex.Message);
                return false;
            }

        }


        public static string ValidFile(string filePath, float sizeInMB, List<string> allowedExt)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);

                long fileSizeInBytes = fileInfo.Length;
                string errMsg = string.Empty;
                if (fileSizeInBytes > sizeInMB * 1024 * 1024)
                {
                    errMsg = " file size validation error";
                }

                string extension = fileInfo.Extension;
                if (!allowedExt.Contains(extension))
                {
                    if (errMsg != string.Empty)
                        errMsg += " and extension error";
                    else
                        errMsg += " extension error";

                }

                return errMsg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "unable validate: " + ex.Message;
            }

        }

        public static void WriteSHA256ChecksumToFile(string inputFilePath, string outputFilePath )
        {
            try
            {
                string sha_256_checksum = SHA256Service.GetSha256Checksum(inputFilePath);

                File.WriteAllText(outputFilePath, sha_256_checksum);
            }
            catch (Exception ex)
            {
                LoggingService.WriteErr(ex.Message);
            }
        }

        public static void DeleteFile(string filePath)
        {
            try
            {
                // Check if the file exists before attempting to delete it
                if (File.Exists(filePath))
                {
                    File.Delete(filePath); // Delete the file
                }
                else
                {
                    Console.WriteLine($"File '{filePath}' not found.");
                }
            }
            catch (Exception ex)
            {
                LoggingService.WriteErr(ex.Message);
            }
        }

        public static void CompressFile(string inputFile, string outputFile)
        {
            // Ensure the input file exists
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"Input file not found: {inputFile}");
            }

            // Use a using statement to ensure streams are properly disposed
            using (FileStream inputFileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream outputFileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (GZipStream gzipStream = new GZipStream(outputFileStream, CompressionMode.Compress))
            {
                // Define a buffer size for reading and writing chunks
                byte[] buffer = new byte[4096]; // 4KB buffer

                int bytesRead;
                // Read from the input file stream and write to the GZip stream
                while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    gzipStream.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
