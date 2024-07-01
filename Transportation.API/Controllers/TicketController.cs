using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;
using Transportation.Core.Constants;
using Transportation.Core.Dto.UserOutput;
using Transportation.Interfaces.IApplicationServices;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Transportation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{Roles.BusStopManager},{Roles.Admin}")]
    public class TicketController(ITicketServices ticketServices) : ControllerBase
    {

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedTicketDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedTicketDto>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-tickets")]
        public async Task<ActionResult> GetAllTickets()
        {
            try
            {
                var time = DateTime.UtcNow;
                var tikets = await ticketServices.GetAllTickets();
                Log.Information($"Get All Tickets Done Successfully ({time} -> {DateTime.UtcNow})");
                return Ok(new ResponseModel<IEnumerable<ReturnedTicketDto>>
                {
                    StatusCode = 200,
                    Body = tikets,
                    Message = "Done"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All Tickets Failed ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = "Failed " + ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedTicketDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-tickets-by-journey/{id:guid}")]
        public async Task<ActionResult> GetAllTicketsByJourneyId([Required] Guid id)
        {
            try
            {
                var tikets = await ticketServices.GetAllTicketsByJourneyId(id);
                Log.Information($"Get All Tickets By Journey Id Done Successfully");
                return Ok(new ResponseModel<IEnumerable<ReturnedTicketDto>>
                {
                    StatusCode = 200,
                    Body = tikets,
                    Message = "Done"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All Tickets By Journey Id Failed ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = "Failed " + ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedTicketDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-all-tickets-by-reserved-time")]
        public async Task<ActionResult> GetAllTicketsByReservedTime([FromRoute] DateTime time)
        {
            try
            {
                var tickets = await ticketServices.GetTicketsByReservedTime(time);
                Log.Information($"Get All Tickets By Reserved Time Done Successfully");

                return Ok(new ResponseModel<IEnumerable<ReturnedTicketDto>>
                {
                    StatusCode = 200,
                    Body = tickets,
                    Message = "Done"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All Tickets By Reserved Time Failed ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = "Failed " + ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedTicketDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [Authorize(Roles = $"{Roles.BusStopManager},{Roles.User},{Roles.Admin}")]
        [HttpGet("get-ticket/{id:guid}")]
        public async Task<ActionResult> GetTicket(Guid id)
        {
            try
            {
                var ticket = await ticketServices.GetTicketById(id);
                Log.Information($"Get Ticket By Id Done Successfully");
                return Ok(new ResponseModel<ReturnedTicketDto>
                {
                    StatusCode = 200,
                    Message = "Done",
                    Body = ticket
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All Tickets By Reserved Time Failed ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = "Failed " + ex.Message,
                    Body = []
                });
            }
        }
    }
}
