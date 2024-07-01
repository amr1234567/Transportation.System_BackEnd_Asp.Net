using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Models;

namespace Transportation.Core.Dto.ServiceInput
{
    public class JourneyDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public DateTime LeavingTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public string DestinationId { get; set; }

        [Required]
        public string StartNusStopId { get; set; }

        [Required]
        public Guid BusId { get; set; }
        public IEnumerable<Ticket> ReservedTickets { get; set; }

        public JourneyHistory ToModel()
        {
            return new JourneyHistory()
            {
                Id = Id,
                DestinationId = DestinationId.ToString(),
                StartBusStopId = StartNusStopId.ToString(),
                BusId = BusId,
                ArrivalTime = ArrivalTime,
                LeavingTime = LeavingTime,
                Tickets = ReservedTickets,
                Date = new DateTime(LeavingTime.Year, LeavingTime.Month, LeavingTime.Day)
            };
        }
    }
}
