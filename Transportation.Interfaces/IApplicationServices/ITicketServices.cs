
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;

namespace Transportation.Interfaces.IApplicationServices
{
    public interface ITicketServices
    {
        Task<IEnumerable<ReturnedTicketDto>> GetAllTickets();
        Task<IEnumerable<ReturnedTicketDto>> GetAllTicketsByJourneyId(Guid id);
        Task<IEnumerable<ReturnedTicketDto>> GetAllTicketsByUserId(string id);
        Task<ReturnedTicketDto> GetTicketById(Guid id);
        Task<IEnumerable<ReturnedTicketDto>> GetTicketsByReservedTime(DateTime dateTime);
        Task<ResponseModel<ReturnedTicketDto>> CutTicket(TicketDto ticketDto, string ConsumerId);
        Task<ResponseModel<ReturnedTicketDto>> BookTicket(TicketDto ticketDto, string ConsumerId);
        Task<IEnumerable<ReturnedTicketDto>> GetAllCutTickets();
        Task<IEnumerable<ReturnedTicketDto>> GetAllBookedTickets();
    }
}
