using Transportation.Core.Dto.ServiceInput;
using Transportation.Core.Models;

namespace Transportation.Core.Helpers.Functions;

public static class JourneysConverters
{
    public static JourneyDto FromUpcomingToHistoryJourneyDto(this UpcomingJourney model) => new()
    {
        Id = model.Id,
        ArrivalTime = model.ArrivalTime,
        BusId = model.BusId,
        DestinationId = model.DestinationId,
        LeavingTime = model.LeavingTime,
        ReservedTickets = model.Tickets,
        StartNusStopId = model.StartBusStopId
    };
}