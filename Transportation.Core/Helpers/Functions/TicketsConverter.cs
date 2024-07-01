using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Models;

namespace Transportation.Core.Helpers.Functions
{
    public static class TicketsConverter
    {
        public static List<ReturnedTicketDto> Convert(this List<Ticket> tickets)
        {
            var returningTickets = new List<ReturnedTicketDto>();
            foreach (var ticket in tickets)
            {
                returningTickets.Add(new ReturnedTicketDto
                {
                    JourneyId = ticket.JourneyId,
                    Price = ticket.Price,
                    SeatNumber = ticket.SeatNum,
                    StartBusStopName = ticket.StartBusStopName,
                    ArrivalTime = ticket.ArrivalTime,
                    DestinationBusStopName = ticket.DestinationName,
                    LeavingTime = ticket.LeavingTime
                });
            }
            return returningTickets;
        }
    }
}
