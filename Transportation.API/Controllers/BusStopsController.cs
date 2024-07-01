using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;
using Transportation.Core.Constants;
using Transportation.Core.Dto.UserOutput;
using Transportation.Interfaces.IIdentityServices;

namespace Transportation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.BusStopManager},{Roles.User},{Roles.Admin}")]
    public class BusStopsController(IManagerServices managerServices) : ControllerBase
    {
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedBusStopDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedBusStopDto>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-related-by-start/{startBusStop}")]
        public async Task<ActionResult> GetAllRelatedBusStops([Required] string startBusStop)
        {
            try
            {
                if (Guid.TryParse(startBusStop, out _))
                    ModelState.AddModelError("StartBusStop", "Id is invalid");
                var records = await managerServices.GetAllDestinationBusStops(startBusStop);
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
                return Ok(new ResponseModel<IEnumerable<BusStopDto>>()
                {
                    Body = [],
                    Message = $"Get All related BusStops Failed :{ex.Message}",
                    StatusCode = 400
                });
            }

        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedBusStopDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedBusStopDto>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all")]
        public async Task<ActionResult> GetAllStartBusStops()
        {
            try
            {
                var records = await managerServices.GetAllStartBusStops();
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
                Log.Error($"Get All BusStops Failed :{ex.Message}");
                return Ok(new ResponseModel<IEnumerable<BusStopDto>>()
                {
                    Body = [],
                    Message = $"Get All BusStops Failed :{ex.Message}",
                    StatusCode = 400
                });
            }

        }

        [ProducesResponseType(typeof(ResponseModel<ReturnedBusStopDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-Bus-stop/{busStopId}")]
        public async Task<ActionResult> GetBusStopById(string busStopId)
        {
            try
            {
                if (Guid.TryParse(busStopId, out _))
                    ModelState.AddModelError("StartBusStop", "Id is invalid");
                var busStop = await managerServices.GetBusStop(busStopId);
                Log.Information("Get BusStop By Id succeeded");
                return Ok(busStop);
            }
            catch (Exception ex)
            {
                Log.Error($"Get  BusStop By Id Failed :{ex.Message}");
                return BadRequest($"Get  BusStop By Id Failed :{ex.Message}");
            }
        }
    }
}
