using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Transportation.Core.Constants;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Models;
using Transportation.Interfaces.IApplicationServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Transportation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.BusStopManager},{Roles.Admin}")]
    public class BusController(IBusServices busService) : ControllerBase
    {

        [ProducesResponseType(typeof(ResponseModel<Bus>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-bus/{busId:guid}")]
        public async Task<ActionResult> GetBus([FromRoute] Guid busId)
        {
            try
            {
                var bus = await busService.GetBusById(busId);
                Log.Information($"get Bus By Id Success: {bus}");
                return Ok(new ResponseModel<Bus>
                {
                    StatusCode = 200,
                    Message = "Done",
                    Body = bus
                });
            }
            catch (Exception ex)
            {
                Log.Error($"get Bus By Id failed: {ex.Message}");
                return BadRequest(new ResponseModel<Bus>
                {
                    StatusCode = 400,
                    Message = $"Get Bus By Id Error occurred :{ex.Message}"
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<Bus>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPut("edit-bus/{busId:guid}")]
        public async Task<ActionResult> EditBus([FromRoute] Guid busId, [FromBody] BusDto model)
        {
            try
            {
                var res = await busService.EditBus(busId, model);
                Log.Information($"edit Bus Success");
                if (res.StatusCode == 200)
                    return Ok(res);
                Log.Error($"edit Bus failed");
                return BadRequest(res);
            }
            catch (Exception ex)
            {
                Log.Error($"edit Bus failed: {ex.Message}");
                return BadRequest($"edit Bus failed: {ex.Message}");
            }
        }
    }
}
