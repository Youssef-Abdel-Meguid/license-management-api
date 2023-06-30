using Microsoft.AspNetCore.Mvc;
using GenerateEncryptedFile.BLL;
using System.Security.Cryptography;
using System.Text;
using GenerateEncryptedFile.Helpers;

namespace GenerateEncryptedFile.Controllers
{
    [ApiController]
    [Route("api/license")]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseBLL licenseBLL;
        private readonly IHelper helper;

        public LicenseController(ILicenseBLL _licenseBLL, IHelper _helper)
        {
            licenseBLL = _licenseBLL;
            helper = _helper;
        }

        [HttpPost("generate")]
        public IActionResult GenerateLicenseFile([FromBody] List<int> componentIds)
        {
            try
            {
                var requestedComponets = licenseBLL.GetRequestedComponentsIds(componentIds);

                var stringBuilder = new StringBuilder();

                foreach (var item in requestedComponets)
                {
                    stringBuilder.AppendLine($"ID: {item.Id}, Name: {item.ComponentName}");
                }

                string fileContent = stringBuilder.ToString();
                string fileName = "license.txt";
                string filePath = Path.Combine(Path.GetTempPath(), fileName);

                byte[] encryptedBytes = helper.EncryptStringToBytes(fileContent, "YourEncryptionKey");

                System.IO.File.WriteAllBytes(filePath, encryptedBytes);

                return File(System.IO.File.OpenRead(filePath), "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                using(MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byte[] encryptedBytes = memoryStream.ToArray();

                    string decryptedContent = helper.DecryptBytesToString(encryptedBytes, "YourEncryptionKey");
                    string decryptedFileName = "decrypted_file.txt";
                    string decryptedFilePath = Path.Combine(Path.GetTempPath(), decryptedFileName);

                    System.IO.File.WriteAllText(decryptedFilePath, decryptedContent);

                    return File(System.IO.File.OpenRead(decryptedFilePath), "application/octet-stream", decryptedFileName);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
