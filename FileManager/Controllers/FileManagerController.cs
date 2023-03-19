using FileManager.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using FileManager.DatabaseAccess;
using Microsoft.AspNetCore.Components.Forms;
using InputFile = FileManager.Models.InputFile;
using System;

namespace FileManager.Controllers
{
    [Route("api/file-management")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly ILogger<FileManagerController> _logger;
        IDatabaseFileCRUD _databaseFileCRUD;

        public FileManagerController(ILogger<FileManagerController> logger, IDatabaseFileCRUD databaseFileCRUD)
        {
            _logger = logger;
            _databaseFileCRUD = databaseFileCRUD;
        }

        [HttpPost("CreateFile")]
        public async Task<IActionResult> CreateFile(IFormFile inputfile)
        {
            // If file does NOT exist, insert it into the database
            if (!_databaseFileCRUD.IsFileExist(inputfile.FileName))
            {
                var inputFile = new InputFile
                {
                    FileName = inputfile.FileName,
                    ContentType = inputfile.ContentType,
                    Length = inputfile.Length,
                    Version = 1
                };

                // If InserFile returns true, it means the database insert happened successfully
                if (_databaseFileCRUD.InsertFile(inputFile))
                {

                    var responseCreated = new FileManagementServiceResponse()
                    {
                        StatusCode = 201,
                        StatusMessage = "The file has been inserted into the database"
                    };

                    return StatusCode(responseCreated.StatusCode, responseCreated.StatusMessage);
                }
            }

            // If file is already inserted, do nothing
            var responseOk = new FileManagementServiceResponse()
            {
                StatusCode = 200,
                StatusMessage = "The file already exits in the database. Nothing got updated."
            };

            return StatusCode(responseOk.StatusCode, responseOk.StatusMessage);
        }

        [HttpPost("UpdateFile")]
        public async Task<IActionResult> UpdateFile(IFormFile inputfile)
        {
            if (inputfile.Length > 0 && _databaseFileCRUD.IsFileExist(inputfile.FileName))
            {
                // Get the latest version of the file in the database
                var latestFileFromDB = _databaseFileCRUD.GetLatestVersionFile(inputfile.FileName);

                if (latestFileFromDB != null)
                {
                    var inputFileToInsert = new InputFile
                    {
                        FileName = latestFileFromDB.FileName,
                        ContentType = inputfile.ContentType, // This attribute comes from the new inserted file
                        Length = inputfile.Length, // This attribute comes from the new inserted file
                        Version = latestFileFromDB.Version + 1 // Increase the version
                    };

                    // If file gets inserted, this will return true
                    if (_databaseFileCRUD.InsertFile(inputFileToInsert))
                    {

                        var responseCreated = new FileManagementServiceResponse()
                        {
                            StatusCode = 201,
                            StatusMessage = "The new version of the file got inserted into the database"
                        };

                        return StatusCode(responseCreated.StatusCode, responseCreated.StatusMessage);
                    }
                }

            }

            var responseBadRequest = new FileManagementServiceResponse()
            {
                StatusCode = 400,
                StatusMessage = "The file does NOT exist in the database. Please insert it first then try to update it"
            };

            return StatusCode(responseBadRequest.StatusCode, responseBadRequest.StatusMessage);
        }

        [HttpPost("DeleteFile")]
        public async Task<IActionResult> DeleteFile()
        {
            // string inputfilename, int? version
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string jsonRequest = await reader.ReadToEndAsync();
                if (!string.IsNullOrEmpty(jsonRequest))
                {
                    DeleteFileBody taskitem = JsonConvert.DeserializeObject<DeleteFileBody>(jsonRequest);

                    // If the version parameter is not provided, put 0 in the version and delete the latest version
                    int fileVersion = (taskitem.Version == null || taskitem.Version == 0) 
                        ? 0 : taskitem.Version.Value;

                    if (_databaseFileCRUD.DeleteFile(taskitem.inputfilename, fileVersion))
                    {
                        var responseDeleted = new FileManagementServiceResponse()
                        {
                            StatusCode = 200,
                            StatusMessage = "The file got deleted"
                        };

                        return StatusCode(responseDeleted.StatusCode, responseDeleted.StatusMessage);
                    }
                }
            }

            var responseDeletFailed = new FileManagementServiceResponse()
            {
                StatusCode = 400,
                StatusMessage = "The file did NOT get deleted because the file with the provided version does NOT exist in the database"
            };

            return StatusCode(responseDeletFailed.StatusCode, responseDeletFailed.StatusMessage);

        }

        [HttpGet("ListFiles")]
        public async Task<IActionResult> ListFiles()
        {
            List<InputFile>  result = _databaseFileCRUD.ListFiles();
            string jsonOutput = JsonConvert.SerializeObject(result);
            return Ok(jsonOutput);
        }

        [HttpGet("ListAllVersionsOfaFile")]
        public async Task<IActionResult> ListAllVersionsOfaFile(string filename)
        {
            List<InputFile> result = _databaseFileCRUD.ListAllVersionsOfAFile(filename);
            string jsonOutput = JsonConvert.SerializeObject(result); 
            return Ok(jsonOutput);
        }
        [HttpGet("ListAllFilesAndVersions")]
        public async Task<IActionResult> ListAllFilesAndVersions()
        {
            List<InputFile> result = _databaseFileCRUD.ListAllFilesAndVersions();
            string jsonOutput = JsonConvert.SerializeObject(result);
            return Ok(jsonOutput);
        }
    }
}
