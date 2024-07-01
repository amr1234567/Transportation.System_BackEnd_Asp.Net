using System.Globalization;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Helpers.Functions;
using Transportation.Core.Models;
using Transportation.Interfaces.IApplicationServices;

namespace Transportation.Services.ApplicationServices
{
    public class UpcomingJourneysServices(
        ISeatServices seatServices,
        ApplicationDbContext context,
        IJourneysHistoryServices journeyHistoryServices,
        IBusServices busServices) : IUpcomingJourneysServices
    {

        public Task<IEnumerable<ReturnedUpcomingJourneyDto>> GetAllUpcomingJourneys()
        {
            var records = context.UpcomingJourneys
                .Include(x => x.Destination)
                .Include(y => y.StartBusStop)
                .AsEnumerable()
                .Select(x => x.FromUpcomingJourneyToReturnedDto());
            return Task.FromResult(records);
        }

        public void TurnUpcomingJourneysIntoHistoryJourneys()
        {
            var records = context.UpcomingJourneys
                .Include(upcomingJourney => upcomingJourney.Tickets)
                .Where(x => x.ArrivalTime < DateTime.UtcNow)
                .ToList();

            foreach (var record in records)
            {
                journeyHistoryServices.AddJourney(record.FromUpcomingToHistoryJourneyDto());
                busServices.SetBusToAvailable(record.BusId);
                seatServices.SetBusSeatsToAvailable(record.BusId);
                context.UpcomingJourneys.Remove(record);
                context.SaveChanges();
            }
        }

        public async Task<UpcomingJourneyDto> AddUpcomingJourney(UpcomingJourneyDto model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (DateTime.UtcNow > model.LeavingTime || DateTime.UtcNow > model.ArrivalTime)
                throw new Exception("Time is in the past");

            var busStop = await context.BusStopMangers.Include(bsm => bsm.BusStops)
                .FirstOrDefaultAsync(bsm => bsm.Id.Equals(model.StartBusStopId));
            if (busStop == null)
                throw new NullReferenceException("StartBus Stop Can't be null");

            var busStopExist = busStop.BusStops != null && busStop.BusStops.Any(bsm =>
                string.Compare(bsm.Id, model.DestinationId, StringComparison.Ordinal) == 0);
            if (!busStopExist)
                throw new NullReferenceException("Destination BusStop Can't be null");

            var numberOfAvailableTickets = context.Seats.Count(s => s.BusId.Equals(model.BusId) && s.IsAvailable);
            //var NumberOfAvailableTickets = _context.Buses.Include(b => b.seats).FirstOrDefault(b => b.Id.Equals(model.BusId)).seats.Count(s => s.IsAvailable);


            await context.UpcomingJourneys.AddAsync(
                model.FromUpcomingJourneyDtoToUpcomingJourney(numberOfAvailableTickets));
            await context.SaveChangesAsync();
            await busServices.SetBusToNotAvailable(model.BusId);
            return model;
        }

        public Task<IEnumerable<ReturnedUpcomingJourneyDto>> GetAllJourneysByStartBusStopId(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("Start BusStopId Can't Be Null");

            var journeys = context.UpcomingJourneys
                .Where(x => x.StartBusStopId.Equals(id)).AsEnumerable()
                .Select(x => x.FromUpcomingJourneyToReturnedDto());
            return Task.FromResult(journeys);
        }

        public Task<IEnumerable<ReturnedUpcomingJourneyDto>> GetAllJourneysByDestinationBusStopId(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("Destination BusStopId Can't Be Null");
            var journeys = context.UpcomingJourneys.Where(x => x.DestinationId.Equals(id));

            var returnedJourneys = journeys
                .Select(j => j.FromUpcomingJourneyToReturnedDto(seatServices.GetAllSeatsInBusByBusId(j.BusId).Result))
                .AsNoTracking().AsEnumerable();
            return Task.FromResult(returnedJourneys);
        }

        public async Task<ReturnedUpcomingJourneyDto> GetJourneyById(Guid id)
        {
            var journey = await context.UpcomingJourneys
                              .Include(uj => uj.StartBusStop)
                              .Include(uj => uj.Destination)
                              .FirstOrDefaultAsync(x => x.Id.Equals(id)) ??
                          throw new Exception($"Journey With {id} doesn't exist");

            return journey.FromUpcomingJourneyToReturnedDto(await seatServices.GetAllSeatsInBusByBusId(journey.BusId));
        }

        public Task<IEnumerable<ReturnedUpcomingJourneyDto>> GetNearestJourneysByBusStopsNames(string destinationId,
            string startBusStopId) // wait
        {
            IQueryable<UpcomingJourney> journeys;

            if (string.IsNullOrEmpty(destinationId) && !string.IsNullOrEmpty(startBusStopId))
            {
                journeys = context.UpcomingJourneys.Where(j => j.StartBusStopId.Equals(startBusStopId));
            }
            else if (string.IsNullOrEmpty(startBusStopId) && !string.IsNullOrEmpty(destinationId))
            {
                journeys = context.UpcomingJourneys.Where(j => j.DestinationId.Equals(destinationId));
            }
            else if (string.IsNullOrEmpty(startBusStopId) && string.IsNullOrEmpty(destinationId))
            {
                journeys = context.UpcomingJourneys;
            }
            else
            {
                journeys = context.UpcomingJourneys.Where(j => j.StartBusStopId.Equals(startBusStopId)
                                                               && j.DestinationId.Equals(destinationId));
            }

            if (journeys is null || !journeys.Any())
                throw new Exception("No Upcoming Journeys");

            var journeysCounted = journeys.OrderBy(b => b.LeavingTime).AsEnumerable();
            return Task.FromResult(journeysCounted.Select(journey =>
                journey.FromUpcomingJourneyToReturnedDto(seatServices.GetAllSeatsInBusByBusId(journey.BusId).Result)));
        }

        public async Task SetArrivalTime(DateTime time, Guid id)
        {
            if (!DateTime.TryParse(time.ToString(CultureInfo.InvariantCulture), out var newArrivalTime) || DateTime.UtcNow >= newArrivalTime)
                throw new Exception("new Date is invalid");
            var journey = await GetUpcomingJourney(id);
            if (journey is null)
                throw new NullReferenceException($"Journey With Id: {id} Doesn't Exist");

            journey.ArrivalTime = newArrivalTime;
            context.UpcomingJourneys.Update(journey);
            await context.SaveChangesAsync();
        }

        public async Task SetLeavingTime(DateTime time, Guid id)
        {

            if (!DateTime.TryParse(time.ToString(CultureInfo.InvariantCulture), out var newArrivalTime) || DateTime.UtcNow >= newArrivalTime)
                throw new Exception("new Date is invalid");
            var journey = await GetUpcomingJourney(id);
            journey.LeavingTime = newArrivalTime;
            context.UpcomingJourneys.Update(journey);
            await context.SaveChangesAsync();
        }

        private async Task<UpcomingJourney> GetUpcomingJourney(Guid id) =>
            await context.UpcomingJourneys.FirstOrDefaultAsync(j => j.Id.Equals(id)) ??
            throw new Exception("Can't get the journey with this id");
    }
}
