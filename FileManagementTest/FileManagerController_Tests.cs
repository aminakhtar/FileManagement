using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace FileManagementTest
{
    [TestClass]
    public class FileManagerController_Tests
    {
        private readonly ILogger<FileManagerController> _logger = new Mock<FileManagerController>();
        IDatabaseFileCRUD _databaseFileCRUD = new Mock<IDatabaseFileCRUD>();

        [TestMethod]
        public void TestMethod1()
        {
            //arrange
            var fileManager = new FileManagerController(_connectDB.Object);

            //setup
            _connectDB.Setup(x => x.ReadTasks()).Returns(GetTestTasks());

            //act
            Task<IActionResult> response = tasksManager.GetAllTasks();

            //assert
            Assert.AreEqual(200, ((ObjectResult)response.Result).StatusCode);
            Assert.AreEqual(GetResponse(GetTestTasks()), ((ObjectResult)response.Result).Value);
        }
    }
}
