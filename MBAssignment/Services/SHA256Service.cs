using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace MBAssignment.Services
{
    public class SHA256Service
    {
        /// <summary>
        /// Computes the SHA256 hash of a given string.
        /// </summary>
        /// <param name="input">The string to hash.</param>
        /// <returns>The SHA256 hash as a hexadecimal string.</returns>
        public static string ComputeSha256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new StringBuilder to collect the bytes and create a string
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data and format each one as a hexadecimal string
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string
                return sBuilder.ToString();
            }
        }

        public static string GetSha256Checksum(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    // Compute the hash of the fileStream
                    byte[] hashValue = sha256.ComputeHash(fileStream);

                    // Convert the byte array to a hexadecimal string
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hashValue)
                    {
                        sb.Append(b.ToString("x2")); // "x2" formats the byte as a two-digit hexadecimal number
                    }
                    return sb.ToString();
                }
            }
        }


        /// <summary>
        /// Verifies if a computed SHA256 hash matches an expected hash for a given string.
        /// </summary>
        /// <param name="input">The original string.</param>
        /// <param name="expectedHash">The expected SHA256 hash in hexadecimal format.</param>
        /// <returns>True if the hashes match, false otherwise.</returns>
        private static bool VerifySha256Hash(string input, string expectedHash)
        {
            // Compute the hash of the input string
            string computedHash = ComputeSha256Hash(input);

            // Compare the computed hash with the expected hash (case-insensitive)
            return StringComparer.OrdinalIgnoreCase.Compare(computedHash, expectedHash) == 0;
        }
    }
}
