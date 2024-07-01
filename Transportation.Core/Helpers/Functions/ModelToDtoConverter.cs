using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Identity;
using Transportation.Core.Models;

namespace Transportation.Core.Helpers.Functions;

public static class ModelToDtoConverter
{
    public static SeatDto FromSeatToSeatDto(this Seat model) => new()
    {
        IsAvailable = model.IsAvailable,
        SeatId = model.SeatId,
        SeatNum = model.SeatNum
    };

    public static ReturnedUpcomingJourneyDto FromUpcomingJourneyToReturnedDto(this UpcomingJourney model,
        IEnumerable<SeatDto>? seats = null) => new()
        {
            ArrivalTime = model.ArrivalTime,
            DestinationName = model.DestinationName,
            LeavingTime = model.LeavingTime,
            NumberOfAvailableTickets = model.NumberOfAvailableTickets,
            StartBusStopName = model.StartBusStopName,
            TicketPrice = model.TicketPrice,
            BusId = model.BusId,
            DestinationId = model.DestinationId,
            StartBusStopId = model.StartBusStopId,
            Id = model.Id,
            JourneyId = model.JourneyId,
            Seats = seats
        };

    public static UpcomingJourney FromUpcomingJourneyDtoToUpcomingJourney(this UpcomingJourneyDto model,
        int numberOfAvailableTickets) => new()
        {
            ArrivalTime = model.ArrivalTime,
            BusId = Guid.Parse(model.BusId),
            DestinationId = model.DestinationId,
            StartBusStopId = model.StartBusStopId,
            TicketPrice = model.TicketPrice,
            LeavingTime = model.LeavingTime,
            NumberOfAvailableTickets = numberOfAvailableTickets,
            JourneyId = Guid.NewGuid(),
        };

    public static ReturnedBusStopDto FromBusStopToBusStopDto(this BusStopManger model) => new()
    {
        Id = model.Id,
        Name = model.Name
    };

    public static ReturnedHistoryJourneyDto FromJourneyHistoryToReturnedDto(this JourneyHistory model) => new()
    {
        ArrivalTime = model.ArrivalTime,
        BusId = model.BusId,
        LeavingTime = model.LeavingTime,
        TicketPrice = model.TicketPrice,
        DestinationName = model.Destination.Name,
        StartBusStopName = model.StartBusStop.Name
    };
}