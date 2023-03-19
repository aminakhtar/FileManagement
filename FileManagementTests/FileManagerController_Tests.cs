using FileManager.Controllers;
using FileManager.DatabaseAccess;
using FileManager.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Text;
using InputFile = FileManager.Models.InputFile;

namespace FileManagementTests
{
    [TestClass]
    public class FileManagerController_Tests
    {
        private Mock<ILogger<FileManagerController>> _logger = new Mock<ILogger<FileManagerController>>();
        private Mock<IDatabaseFileCRUD> _databaseFileCRUD = new Mock<IDatabaseFileCRUD>();

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestCreateFile(bool fileInserted)
        {
            //arrange
            string fileName = "TestFile.jpg";
            IFormFile inputfile = GetFileMock(fileName,"image/jpeg", "Test Content");

            //setup
            _databaseFileCRUD.Setup(x => x.IsFileExist(fileName)).Returns(false);
            _databaseFileCRUD.Setup(x => x.InsertFile(It.IsAny<InputFile>())).Returns(fileInserted);

            var fileManagerController = new FileManagerController(_logger.Object, _databaseFileCRUD.Object);

            //act
            Task<IActionResult> response = fileManagerController.CreateFile(inputfile);

            //assert
            if (fileInserted)
            {
                Assert.AreEqual(201, ((ObjectResult)response.Result).StatusCode);
                Assert.AreEqual("The file has been inserted into the database", ((ObjectResult)response.Result).Value);
            }
            else
            {
                Assert.AreEqual(200, ((ObjectResult)response.Result).StatusCode);
                Assert.AreEqual("The file already exits in the database. Nothing got updated", ((ObjectResult)response.Result).Value);

            }
        }
        private IFormFile GetFileMock(string fileName, string contentType, string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            var file = new FormFile(
                baseStream: new MemoryStream(bytes),
                baseStreamOffset: 0,
                length: bytes.Length,
                name: fileName,
                fileName: fileName
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return file;
        }
    }
}