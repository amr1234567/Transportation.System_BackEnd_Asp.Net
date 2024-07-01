using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Helpers.Functions;
using Transportation.Core.Models;
using Transportation.Interfaces.IApplicationServices;
using Exception = System.Exception;

namespace Transportation.Services.ApplicationServices
{
    public class TicketServices(ApplicationDbContext context, ISeatServices seatServices) : ITicketServices
    {
        public async Task<ResponseModel<ReturnedTicketDto>> CutTicket(TicketDto ticketDto, string consumerId)
        {
            ArgumentNullException.ThrowIfNull(ticketDto);
            ArgumentNullException.ThrowIfNull(consumerId);

            var ticket = await GenerateTicket(ticketDto, consumerId, false);
            return new ResponseModel<ReturnedTicketDto>
            {
                StatusCode = 200,
                Message = "Ticket Cut Successfully",
                Body = ticket
            };
        }

        public async Task<ResponseModel<ReturnedTicketDto>> BookTicket(TicketDto ticketDto, string userId)
        {

            ArgumentNullException.ThrowIfNull(ticketDto);
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("userId can't be empty");
            }

            var ticket = await GenerateTicket(ticketDto, userId, true);
            return new ResponseModel<ReturnedTicketDto>
            {
                StatusCode = 200,
                Message = "Ticket Booked Successfully",
                Body = ticket
            };
        }

        public Task<IEnumerable<ReturnedTicketDto>> GetAllTickets()
        {
            var tickets = context.Tickets.Select(t => t.ConvertToDto()).AsNoTracking().AsEnumerable();

            return Task.FromResult(tickets);
        }


        public Task<IEnumerable<ReturnedTicketDto>> GetAllTicketsByJourneyId(Guid id)
        {
            var tickets = context.Tickets.Where(t => t.JourneyId.Equals(id))
                .Select(t => t.ConvertToDto()).AsNoTracking().AsEnumerable();

            return Task.FromResult(tickets);
        }

        public Task<IEnumerable<ReturnedTicketDto>> GetAllTicketsByUserId(string id)
        {
            var tickets = context.Tickets.Where(x => x.ConsumerId == id)
                .Select(t => t.ConvertToDto()).AsNoTracking().AsEnumerable();
            return Task.FromResult(tickets);
        }

        public Task<IEnumerable<ReturnedTicketDto>> GetAllBookedTickets()
        {
            var tickets = context.Tickets.Where(x => x.ReservedOnline)
                .Select(t => t.ConvertToDto()).AsNoTracking().AsEnumerable();
            return Task.FromResult(tickets);
        }

        public Task<IEnumerable<ReturnedTicketDto>> GetAllCutTickets()
        {
            var tickets = context.Tickets.Where(x => !x.ReservedOnline)
                .Select(t => t.ConvertToDto()).AsNoTracking().AsEnumerable();
            return Task.FromResult(tickets);
        }

        public async Task<ReturnedTicketDto> GetTicketById(Guid id)
        {
            var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id.Equals(id)) ??
                         throw new Exception("Ticket Can't be found");
            return ticket.ConvertToDto();
        }


        public Task<IEnumerable<ReturnedTicketDto>> GetTicketsByReservedTime(DateTime dateTime)
        {
            var tickets = context.Tickets.Where(x => x.CreatedTime >= dateTime)
                .Select(t => t.ConvertToDto())
                .AsNoTracking().AsEnumerable();

            return Task.FromResult(tickets);
        }


        private async Task<ReturnedTicketDto> GenerateTicket(TicketDto ticketDto, string consumerId, bool online)
        {
            ArgumentNullException.ThrowIfNull(ticketDto);
            ArgumentNullException.ThrowIfNull(consumerId);

            var seat = await context.Seats.FindAsync(Guid.Parse(ticketDto.SeatId));

            if (seat == null)
                throw new Exception("Seat Doesn't Exist");

            if (!seat.IsAvailable)
                throw new Exception("Seat Not Available");

            var journey = await context.UpcomingJourneys.Include(j => j.Destination)
                .Include(j => j.StartBusStop)
                .FirstOrDefaultAsync(j => j.Id.Equals(Guid.Parse(ticketDto.JourneyId)));

            if (journey == null)
                throw new Exception("journey can't be Found");

            var ticket = GenerateTicketFromDetails(ticketDto, journey, seat, consumerId, online);

            await seatServices.ReserveSeat(Guid.Parse(ticketDto.SeatId));
            await context.Tickets.AddAsync(ticket);

            await context.SaveChangesAsync();
            return ticket.ConvertToDto();
        }

        private static Ticket GenerateTicketFromDetails(TicketDto ticketDto, UpcomingJourney journey, Seat seat,
            string consumerId, bool online) => new()
            {
                Id = Guid.NewGuid(),
                CreatedTime = DateTime.UtcNow,
                SeatNum = seat.SeatNum,
                UpcomingJourneyId = Guid.Parse(ticketDto.JourneyId),
                ConsumerId = consumerId,
                ReservedOnline = online,
                Price = journey.TicketPrice,
                JourneyId = Guid.Parse(ticketDto.JourneyId),
                ArrivalTime = journey.ArrivalTime,
                DestinationId = journey.DestinationId,
                DestinationName = journey?.Destination?.Name,
                LeavingTime = journey.LeavingTime,
                StartBusStopId = journey.StartBusStopId,
                StartBusStopName = journey?.StartBusStop?.Name
            };
    }
}
