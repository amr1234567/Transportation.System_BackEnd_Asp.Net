using Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Transportation.Core.Constants;
using Transportation.Core.Dto.Identity;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Models;
using Transportation.Interfaces.IApplicationServices;
using Transportation.Interfaces.IIdentityServices;

namespace Transportation.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Admin)]
    [ApiController]
    public class AdminController(IBusServices busService, IAdminServices adminServices, IManagerServices managerServices, IJourneysHistoryServices journeysHistoryServices) : ControllerBase
    {

        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [NonAction]
        [AllowAnonymous]
        [HttpPost("sign-up")]
        public async Task<ActionResult> SignUp(SignUpAsAdminDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var response = await adminServices.SignUp(model);
                if (response)
                {
                    Log.Information($"Sign up Succeeded");
                    return Ok(new ResponseModel<string>
                    {
                        StatusCode = 200,
                        Message = "Every thing is good"
                    });
                }
                Log.Error($"Sign up Failed");
                return BadRequest(new ResponseModel<string>
                {
                    StatusCode = 400,
                    Message = "Wrong Email Or Password"
                });

            }
            catch (Exception ex)
            {
                Log.Error($"Sign up Failed");
                return BadRequest(new ResponseModel<string>
                {
                    StatusCode = 400,
                    Message = ex.Message
                });
            }
        }

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
                var response = await adminServices.SignIn(model);
                if (response.StatusCode == 200)
                {
                    Log.Information($"Sign in Succeeded");
                    return Ok(response);
                }
                Log.Error($"Sign in Failed");
                return BadRequest(new ResponseModel<string>
                {
                    StatusCode = 400,
                    Message = "Bad Input"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Sign in Failed ({ex.Message})");
                return BadRequest(new ResponseModel<TokenModel>
                {
                    StatusCode = 400,
                    Message = ex.Message
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("create-manager")]
        public async Task<ActionResult> CreateManager(SignUpAsManagerDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var response = await managerServices.SignUp(model);
                if (response)
                {
                    Log.Information($"Manager Created");
                    return Ok(new ResponseModel<bool>
                    {
                        StatusCode = 200,
                        Message = "Every thing is good",
                        Body = true
                    });
                }
                Log.Error($"Manager Creation Failed");
                return BadRequest(new ResponseModel<bool>
                {
                    StatusCode = 400,
                    Message = "Bad"
                });

            }
            catch (Exception ex)
            {
                Log.Error($"Manager Creation Failed ({ex.Message})");
                return BadRequest(new ResponseModel<string>
                {
                    StatusCode = 400,
                    Message = ex.Message
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("enroll-bus-stop-to-bus-stop/{startBusStopId}/{destinationBusStopId}")]
        public async Task<ActionResult> EnrollBusStopToAnother(string startBusStopId, string destinationBusStopId)
        {
            try
            {
                await managerServices.enrollBusStop(startBusStopId, destinationBusStopId);
                Log.Information($"Enrolled Process Succeeded");
                return Ok(new ResponseModel<bool>
                {
                    StatusCode = 200,
                    Body = true,
                    Message = "BusStop Added Successfully"

                });
            }
            catch (Exception ex)
            {
                Log.Error($"Enrolled Process Failed ({ex.Message})");
                return BadRequest(new ResponseModel<bool>
                {
                    StatusCode = 400,
                    Body = false,
                    Message = ex.Message
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedHistoryJourneyDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedHistoryJourneyDto>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-history-journeys")]
        public async Task<ActionResult> GetAllJourneysInDb()
        {
            try
            {
                var time = DateTime.UtcNow;
                var historyJourneys = await journeysHistoryServices.GetAllJourneys();
                Log.Information($"Get All Journeys Succeeded({time} -> {DateTime.UtcNow})");
                return Ok(new ResponseModel<IEnumerable<ReturnedHistoryJourneyDto>>
                {
                    StatusCode = 200,
                    Body = historyJourneys,
                    Message = "All Journeys"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All Journeys Failed ({ex.Message})");
                return BadRequest(new ResponseModel<List<ReturnedHistoryJourneyDto>>
                {
                    StatusCode = 400,
                    Message = ex.Message
                });
            }
        }
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Bus>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Bus>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-buses")]
        public async Task<ActionResult> GetAllBuses()
        {
            try
            {
                var buses = await busService.GetAllBusesWithSeats();
                var model = new ResponseModel<IEnumerable<Bus>>
                {

                    StatusCode = 200,
                    Message = "Done",
                    Body = buses

                };
                Log.Information("Get All Buses Success");
                return Ok(model);
            }
            catch (Exception ex)
            {
                var model = new ResponseModel<IEnumerable<Bus>>
                {
                    StatusCode = 404,
                    Message = ex.Message,
                    Body = []
                };
                Log.Error($"Get All Buses Error:{ex.Message}");
                return BadRequest(model);
            }

        }
    }
}
