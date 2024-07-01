using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Transportation.Core.Constants;
using Transportation.Core.Dto.UserOutput;
using Transportation.Interfaces.IApplicationServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Transportation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.BusStopManager},{Roles.User},{Roles.Admin}")]
    public class UpcomingJourneyController(IUpcomingJourneysServices upcomingJourneysServices) : ControllerBase
    {


        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpGet("get-all-upcoming-journeys")]
        public async Task<ActionResult> GetAllUpcomingJourneys()
        {
            try
            {
                var journeys = await upcomingJourneysServices.GetAllUpcomingJourneys();
                Log.Information($"Get All Journeys Succeeded");
                return Ok(new ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>
                {
                    StatusCode = 200,
                    Body = journeys,
                    Message = "Done"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All Journeys failed {ex.Message}");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    Message = ex.Message,
                    StatusCode = 400,
                    Body = []
                });
            }

        }

        [ProducesResponseType(typeof(ResponseModel<ReturnedUpcomingJourneyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-upcoming-journey/{upcomingJourneyId:guid}")]
        public async Task<ActionResult> GetJourneyById(Guid upcomingJourneyId)
        {
            try
            {
                var journey = await upcomingJourneysServices.GetJourneyById(upcomingJourneyId);
                Log.Information($"Get Journey By Id Succeeded");
                return Ok(new ResponseModel<ReturnedUpcomingJourneyDto>
                {
                    StatusCode = 200,
                    Body = journey,
                    Message = "Done"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get Journey By Id failed {ex.Message}");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = $"Get Journey Failed {ex.Message}",
                    Body = []
                });
            }

        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-Nearest-upcoming-journey/{destinationId}/{startBusStopId}")]
        public async Task<ActionResult> GetNearestJourneyByDestination(string destinationId, string startBusStopId)
        {
            try
            {
                var journey = await upcomingJourneysServices.GetNearestJourneysByBusStopsNames(destinationId, startBusStopId);
                Log.Information($"Get Nearest Journey By Destination Succeeded");
                return Ok(new ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>
                {
                    StatusCode = 200,
                    Body = journey,
                    Message = "Done"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get Nearest Journey By Destination failed {ex.Message}");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = $"Get Nearest Journey By Destination failed  {ex.Message}",
                    Body = []
                });
            }

        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-by-destination/{destinationId}")]
        public async Task<ActionResult> GetAllJourneysByDestinationBusStopId(string destinationId)
        {
            try
            {
                var journeys = await upcomingJourneysServices.GetAllJourneysByDestinationBusStopId(destinationId);
                Log.Information($"Get All Journeys By Destination Bus Stop Id Succeeded");
                return Ok(new ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>
                {
                    StatusCode = 200,
                    Body = journeys,
                    Message = "Done"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All Journeys By Destination Bus Stop Id failed {ex.Message}");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = $"Get All Journeys By Destination Bus Stop Id failed {ex.Message}",
                    Body = []
                });
            }

        }

        [HttpGet("get-all-by-start/{startBusStopId}")]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllJourneysByStartBusStopId(string startBusStopId)
        {
            try
            {
                var journeys = await upcomingJourneysServices.GetAllJourneysByStartBusStopId(startBusStopId);
                Log.Information($"Get All Journeys By Start Bus Stop Id Succeeded");
                return Ok(new ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>
                {
                    StatusCode = 200,
                    Body = journeys,
                    Message = "Done"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All Journeys By Start Bus Stop Id failed {ex.Message}");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = $"Get All Journeys By Start Bus Stop Id failed {ex.Message}",
                    Body = []
                });
            }
        }
    }
}
