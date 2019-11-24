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
            Summary[] summaries = new Summary[]
            {
                new Summary
                {
                    MaxVolume = 50,
                    MaxWeight = 50,
                    NumberOfUnassignedOrders = 50,
                    NumberOfVisits = 5,
                    TotalDistance = 45,
                    ServiceTime = System.DateTime.ParseExact("05:21:50","H:m:s", null),
                    TravellingTime = System.DateTime.ParseExact("05:21:50","H:m:s", null),
                    WaitingTime = System.DateTime.ParseExact("05:21:50","H:m:s", null),
                    VehicleName = "v1"
                },

                new Summary
                {
                    MaxVolume = 150,
                    MaxWeight = 5,
                    NumberOfUnassignedOrders = 450,
                    NumberOfVisits = 5,
                    TotalDistance = 45,
                    ServiceTime = System.DateTime.ParseExact("05:21:50","H:m:s", null),
                    TravellingTime = System.DateTime.ParseExact("05:21:50","H:m:s", null),
                    WaitingTime = System.DateTime.ParseExact("05:21:50","H:m:s", null),
                    VehicleName = "v2"
                }
            };

            Itineraries[] itineraries = new Itineraries[]
            {
                new Itineraries
                {
                   LocationName = "Location 1",
                   OrderName ="Order 1",
                   VehicleName = "Vehicle 1"
                },

                new Itineraries
                {
                   LocationName = "Location 2",
                   OrderName ="Order 2",
                   VehicleName = "Vehicle 2"
                }
            };

            return new FileOutput
            {
                Summaries = summaries,
                Itineraries = itineraries
            };
        }
    }
}
