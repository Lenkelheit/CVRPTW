using API.Services;
using API.Validation;

using Domains.Models.Input;
using Domains.Models.Output;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutingController : ControllerBase
    {
        private readonly FileService fileService;
        private readonly MessageService messageService;

        public RoutingController(FileService fileService, MessageService messageService)
        {
            this.fileService = fileService;
            this.messageService = messageService;
        }

        [HttpPost]
        public ActionResult<FileInput> Post([AllowedExtensions(new [] { ".xlsx" })]IFormFile file)
        {
            FileInput fileData; 
            try
            {
                fileData = fileService.Parse(file.OpenReadStream());

                messageService.SendFileData(fileData);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(fileData);
        }

        [HttpGet]
        public FileContentResult Download()
        {
            string fileName = "Results.xlsx";
            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            FileOutput fileOutput = messageService.DequeueData();
            byte[] fileContent = fileService.Save(fileOutput);

            return File(fileContent, fileType, fileName);
        }
    }
}
