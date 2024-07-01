using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
using Transportation.Core.Constants;
using Transportation.Core.Dto.Identity;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Interfaces.IApplicationServices;
using Transportation.Interfaces.IIdentityServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Transportation.API.Controllers
{
    //[ValidationFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserServices userServices, ITicketServices ticketServices) : ControllerBase
    {
        private readonly IUserServices _userServices = userServices;
        private readonly ITicketServices _ticketServices = ticketServices;

        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("sign-up")]
        public async Task<ActionResult> SignUp([FromBody] SignUpDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                    {
                        StatusCode = 400,
                        Message = "Invalid Input",
                        Body = ModelState.Keys.Select(key => new ErrorModelState(key, ModelState[key].Errors.Select(x => x.ErrorMessage).ToList()))
                    });
                }
                var response = await _userServices.SignUp(model);
                if (response.StatusCode != 200)
                    return BadRequest(response);
                Log.Information($"Sign Up Done Successfully");
                return Ok(response);

                #region Confirm With Email
                //var url = Url.Action(nameof(ConfirmEmail), "Authentication");
                //var res = await _userServices.SignUp(model, url);

                //return res ? Ok("All is good") : BadRequest("Something went Wrong"); 
                #endregion
            }
            catch (Exception ex)
            {
                Log.Error($"Sign Up Failed ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = "Failed " + ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [NonAction]
        [HttpPost("confirm-email")]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            try
            {
                var res = await _userServices.ConfirmEmail(email, token);

                Log.Information($"Confirming email succeeded");

                return res ? Ok("Ur Email Has Been Verify") : BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>()
                {
                    Body = [],
                    StatusCode = 400,
                    Message = "Wrong Email Token"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Confirming email has failed ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>()
                {
                    Body = [],
                    StatusCode = 400,
                    Message = ex.Message
                });
            }
        }


        [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("confirm-account")]
        public async Task<ActionResult> ConfirmPhoneNumber([FromBody] ConfirmPhoneNumberDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var res = await _userServices.ConfirmPhoneNumber(model.Email, model.PhoneNumber, model.VerificationCode);
                Log.Information($"Confirming phone number succeeded");

                return res ? Ok(new ResponseModel<string>
                {
                    Message = "Ur PhoneNumber Has Been Verify",
                    StatusCode = 200
                }) : BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    Message = "Something went Wrong",
                    StatusCode = 400,
                    Body = []
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Something went Wrong ({ex.Message})");
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    Message = $"Confirming phone number has failed ({ex.Message})",
                    StatusCode = 400,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<TokenModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("sign-in")]
        public async Task<ActionResult> SignIn([FromBody] LogInDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var response = await _userServices.SignIn(model);

                switch (response.StatusCode)
                {
                    case 200:
                        Log.Information($"Sign In Done successfully");
                        return Ok(response);
                    case 400:
                        return BadRequest(response);
                    case 404:
                        return NotFound(response);
                    default:
                        return Unauthorized(new ResponseModel<TokenModel>
                        {
                            StatusCode = 401,
                            Message = response.Message,
                            Body = response.Body
                        });
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Sign In Failed ({ex.Message})");

                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ReturnedTicketDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = $"{Roles.User},{Roles.BusStopManager}")]
        [HttpGet("get-all-tickets-by-user")]
        public async Task<ActionResult> GetAllTicketsByUserId()
        {
            try
            {
                var tickets = await _ticketServices.GetAllTicketsByUserId(GetUserIdFromClaims());
                Log.Information($"Get All Tickets By User Id Done successfully");

                return Ok(new ResponseModel<IEnumerable<ReturnedTicketDto>>
                {
                    StatusCode = 200,
                    Body = tickets,
                    Message = "Done"
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Get All Tickets By User Id Failed  ({ex.Message})");

                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = Roles.User)]
        [HttpPost("book-ticket")]
        public async Task<ActionResult> BookTicket(TicketDto model)
        {
            try
            {
                if (!Guid.TryParse(model.JourneyId, out _))
                    ModelState.AddModelError("JourneyId", "Journey Id Must be a valid Guid");
                if (!Guid.TryParse(model.SeatId, out _))
                    ModelState.AddModelError("SeatId", "Seat Id Must be a valid Guid");

                if (!ModelState.IsValid)
                    return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                    {
                        StatusCode = 400,
                        Message = "Input is invalid",
                        Body = ModelState.Keys.Select(key => new ErrorModelState(key, ModelState[key].Errors.Select(x => x.ErrorMessage).ToList()))
                    });
                var ticket = await _ticketServices.BookTicket(model, GetUserIdFromClaims());
                Log.Information($"Book Ticket Done successfully");

                return ticket.StatusCode == 200 ? Ok(ticket) : BadRequest(ticket);
            }
            catch (Exception ex)
            {
                Log.Error($"Book Ticket Failed  ({ex.Message})");

                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("forget-password")]
        public async Task<ActionResult> ForgetPasswordVerify([FromBody] ForgetPasswordVerifyDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                    {
                        StatusCode = 400,
                        Message = "Invalid Input",
                        Body = ModelState.Keys.Select(key => new ErrorModelState(key, ModelState[key].Errors.Select(x => x.ErrorMessage).ToList()))
                    });
                var response = await _userServices.ResetPassword(model.Email);
                if (response.StatusCode == 200)
                {
                    Log.Information($"Forget Password Verification Done successfully");

                    return Ok(response);
                }
                Log.Information($"Forget Password Verification Failed, Please Try Again");

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Log.Error($"ForgetPasswordVerify Failed  ({ex.Message})");

                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [HttpPost("reset-Password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Bad model");
                var _ = await _userServices.ResetPasswordConfirmation(model);

                Log.Information($"reset Password  Done successfully");

                return Ok(new ResponseModel<bool>
                {
                    Message = "Password has been reset",
                    StatusCode = 200,
                    Body = true
                });
            }
            catch (Exception ex)
            {
                Log.Error($"ForgetPasswordVerify Failed  ({ex.Message})");

                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                {
                    StatusCode = 400,
                    Message = ex.Message,
                    Body = []
                });
            }
        }

        [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<ErrorModelState>>), StatusCodes.Status400BadRequest)]
        [Authorize(Roles = Roles.User)]
        [HttpPost("edit-personal-data")]
        public async Task<ActionResult> EditPersonalData(EditPersonalDataDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>
                    {
                        StatusCode = 400,
                        Message = "Invalid Input",
                        Body = ModelState.Keys.Select(key => new ErrorModelState(key, ModelState[key].Errors.Select(x => x.ErrorMessage).ToList()))
                    });
                }
                var response = await _userServices.EditPersonalData(model, GetUserIdFromClaims());
                return response.StatusCode switch
                {
                    200 => Ok(response),
                    400 => BadRequest(response),
                    _ => StatusCode(response.StatusCode, response)
                };
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseModel<IEnumerable<ErrorModelState>>()
                {
                    StatusCode = 400,
                    Message = e.Message,
                    Body = []
                });
            }
        }

        private string GetUserIdFromClaims()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
