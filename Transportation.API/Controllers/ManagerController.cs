using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Hangfire;
using Serilog;
using Transportation.Interfaces.IApplicationServices;
using Transportation.Interfaces.IIdentityServices;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.Identity;
using Transportation.Core.Constants;

namespace Transportation.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.BusStopManager)]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status401Unauthorized)]
    [ApiController]
    public class ManagerController(IBusServices busServices, ITicketServices ticketServices, IManagerServices managerServices, IUpcomingJourneysServices upcomingJourneysServices) : ControllerBase
    {

        [ProducesResponseType(typeof(ResponseModel<TokenModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<ActionResult> SignIn(LogInDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var response = await managerServices.SignIn(model);

                Log.Information($"Sign In Succeeded");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error($"Can't Sign In ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-upcoming-journeys")]
        public async Task<ActionResult> GetAllJourneys()
        {
            try
            {
                var journeys = await upcomingJourneysServices.GetAllJourneysByStartBusStopId(GetManagerIdFromClaims());
                Log.Information($"GetAllJourneys Succeeded");
                return Ok(new ResponseModel<IEnumerable<ReturnedUpcomingJourneyDto>>
                {
                    Body = journeys,
                    Message = "ALl Journeys",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                Log.Error($"GetAllJourneys Failed ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    Message = ex.Message,
                    StatusCode = 400,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("add-bus")]
        public async Task<ActionResult> AddBus([FromBody] BusDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                    {
                        StatusCode = 400,
                        Message = "Error",
                        Body = ModelState.Keys.Select(key => new ErrorModelState(key, ModelState[key]!.Errors.Select(x => x.ErrorMessage).ToList()))
                    });
                await busServices.AddBus(model);
                Log.Information($"AddBus Succeeded");
                return Ok(new ResponseModel<bool>
                {
                    StatusCode = 200,
                    Message = "Bus Added",
                    Body = true
                });
            }
            catch (Exception ex)
            {
                Log.Error($"AddBus Failed ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 200,
                    Message = ex.Message,
                    Body = []
                });
            }
        }


        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("add-journey")]
        public async Task<ActionResult> AddJourney([FromBody] UpcomingJourneyDto model)
        {
            try
            {
                if (!Guid.TryParse(model.BusId, out Guid busId))
                    ModelState.AddModelError("busId", "Bus Id Must be a valid Guid");
                if (!Guid.TryParse(model.DestinationId, out _))
                    ModelState.AddModelError("destinationId", "Destination Id Must be a valid Guid");

                if (!ModelState.IsValid)
                    return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                    {
                        StatusCode = 400,
                        Message = "Error",
                        Body = ModelState.Keys.Select(key => new ErrorModelState(key, ModelState[key].Errors.Select(x => x.ErrorMessage).ToList()))
                    });

                model.StartBusStopId = GetManagerIdFromClaims();
                var bus = await busServices.GetBusById(busId);
                if (bus.IsAvailable == false)
                {
                    Log.Error($"AddBus Failed (bus not available)");
                    return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                    {
                        Body = [],
                        Message = "bus on another journey added",
                        StatusCode = 400
                    });
                }
                await upcomingJourneysServices.AddUpcomingJourney(model);

                var duration = model.ArrivalTime - DateTime.UtcNow;

                BackgroundJob.Schedule(() => upcomingJourneysServices.TurnUpcomingJourneysIntoHistoryJourneys(), duration);

                Log.Information($"AddJourney Succeeded");
                return Ok(new ResponseModel<bool>
                {
                    Body = true,
                    Message = "Journey added",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                Log.Error($"AddJourney Failed ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>?>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    Body = []
                });
            }
        }


        [ProducesResponseType(typeof(ResponseModel<ReturnedTicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("cut-ticket")]
        public async Task<ActionResult> CutTicket(TicketDto model)
        {
            try
            {
                if (!Guid.TryParse(model.JourneyId.ToString(), out _))
                    ModelState.AddModelError("JourneyId", "Journey Id Must be a valid Guid");
                if (!Guid.TryParse(model.SeatId.ToString(), out _))
                    ModelState.AddModelError("SeatId", "Seat Id Must be a valid Guid");

                if (!ModelState.IsValid)
                    return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                    {
                        StatusCode = 400,
                        Message = "Input is invalid",
                        Body = ModelState.Keys.Select(key => new ErrorModelState(key, ModelState[key]!.Errors.Select(x => x.ErrorMessage).ToList()))
                    });

                var ticket = await ticketServices.CutTicket(model, GetManagerIdFromClaims());
                Log.Information($"CutTicket Succeeded");
                return ticket.StatusCode == 200 ? Ok(ticket) : BadRequest(ticket);
            }
            catch (Exception ex)
            {
                Log.Error($"CutTicket Failed");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    Message = ex.Message,
                    StatusCode = 400,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedBusStopDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedBusStopDto>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-related-by-manager")]
        public async Task<ActionResult> GetAllRelatedBusStops()
        {
            try
            {
                var records = await managerServices.GetAllDestinationBusStops(GetManagerIdFromClaims());
                Log.Information("Get All BusStops succeeded");
                return Ok(new ResponseModel<IEnumerable<ReturnedBusStopDto>>()
                {
                    Body = records,
                    Message = "Done",
                    StatusCode = 200
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All related BusStops Failed :{ex.Message}");
                return Ok(new ResponseModel<IEnumerable<ErrorModelState>>()
                {
                    Body = [],
                    Message = $"Get All related BusStops Failed :{ex.Message}",
                    StatusCode = 400
                });
            }

        }

        private ActionResult TurnUpcomingJourneysIntoHistoryJourneys()
        {
            try
            {
                upcomingJourneysServices.TurnUpcomingJourneysIntoHistoryJourneys();
                Log.Information($"Empty the Upcoming journeys Succeeded");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"Empty the Upcoming journeys Failed ({ex.Message})");
                return BadRequest(ex.Message);
            }
        }

        private string GetManagerIdFromClaims()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
