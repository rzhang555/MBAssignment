using Xunit;
using MBAssignment.Services;
using System.Text;

namespace MBAssignment.xUnit.Tests
{
    public class UnitTestsForMBFileProcessingServices
    {
        [Fact]
        public void Test_SHA256Checksum_True()
        {           
            string str = "This is for Multibeam test.";
            string expectd_sha256_checksum = "03da1a5051f672775ee5b4efbbb51010f2f303953cd6d0fd00631f5aa79f1fae";

            string result = SHA256Service.ComputeSha256Hash(str);

            Assert.Equal(expectd_sha256_checksum, result);            
        }

        [Fact]
        public void Test_SHA256Checksum_False()
        { 
            //chang "." to "!" in test string
            string str = "This is for Multibeam test!";  
            string expectd_sha256_checksum = "03da1a5051f672775ee5b4efbbb51010f2f303953cd6d0fd00631f5aa79f1fae";

            string result = SHA256Service.ComputeSha256Hash(str);

            Assert.False(expectd_sha256_checksum == result);
        }

        [Fact] 
        public void Test_file_validation_True()
        {
            string fileName = "testfile.txt";
            string fileContent = "For MultiBeam unit test: this is some test content for the file.";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            float maxSizeInMB = 1.0f;
            List<string> allowedExt = new List<string>() { ".txt" };

            // Act - Create a temporary file for testing
            File.WriteAllBytes(fileName, fileBytes);

            // Assert
            FileInfo fileInfo = new FileInfo(fileName);
            Assert.True(fileInfo.Exists); // Verify the file was created
            Assert.True(FileService.ValidFile(fileName, maxSizeInMB, allowedExt) == string.Empty); 

            // Clean up - Delete the temporary file
            File.Delete(fileName);
        }

        [Fact]
        public void Test_file_ext_validation_False()
        {
            string fileName = "testfile2.txt";
            string fileContent = "For MultiBeam unit test: this is some test content for the file.";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            float maxSizeInMB = 1.0f;
            List<string> allowedExt = new List<string>() { ".doc" };

            // Act - Create a temporary file for testing
            File.WriteAllBytes(fileName, fileBytes);

            // Assert
            FileInfo fileInfo = new FileInfo(fileName);
            Assert.True(fileInfo.Exists); // Verify the file was created
            Assert.False(FileService.ValidFile(fileName, maxSizeInMB, allowedExt) == string.Empty); 

            // Clean up - Delete the temporary file
            File.Delete(fileName);
        }
    }
}