using API.Validation;
using API.Controllers.Services;

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
        FileService fileService;

        public RoutingController(FileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpPost]
        public ActionResult<FileInput> Post([AllowedExtensions(new string[] { ".xlsx" })]IFormFile file)
        {
            FileInput fileData; 
            try
            {
                fileData = fileService.Parse(file.OpenReadStream());
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
            FileOutput fileOutput = MockData();
            byte[] fileContent = fileService.Save(fileOutput);

            return File(
                fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        private FileOutput MockData()
        {
            Summary[] summaries = new []
            {
                new Summary
                {
                    VehicleName = "v1",
                    Distance = 122,
                    Load = 15,
                    NumberOfVisits = 5,
                    Time = 45
                },

                new Summary
                {
                    VehicleName = "v2",
                    Distance = 122,
                    Load = 15,
                    NumberOfVisits = 5,
                    Time = 45
                }
            };

            Itineraries[] itineraries = new []
            {
                new Itineraries
                {
                   VehicleName = "Vehicle 1",
                   Distance = 45,
                   Load = 5,
                   From = System.DateTime.Now,
                   To = System.DateTime.Now
                },

                new Itineraries
                {
                    VehicleName = "Vehicle 2",
                    Distance = 45,
                    Load = 5,
                    From = System.DateTime.Now,
                    To = System.DateTime.Now
                }
            };

            Dropped[] droppedLocations = new[]
            {
                new Dropped
                {
                    LocationName = "Dropped 1"
                }
            };
            Totals[] totals = new[]
            {
                new Totals
                {
                    Distance = 45,
                    Load = 45,
                    Time = 54
                }
            };

            return new FileOutput
            {
                Summaries = summaries,
                Itineraries = itineraries,
                DroppedLocation = droppedLocations,
                Totals = totals
            };
        }
    }
}
