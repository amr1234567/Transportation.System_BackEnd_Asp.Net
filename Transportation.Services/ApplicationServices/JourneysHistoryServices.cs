using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Transportation.Core.Dto.ServiceInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Helpers.Functions;
using Transportation.Core.Models;
using Transportation.Interfaces.IApplicationServices;

namespace Transportation.Services.ApplicationServices
{
    public class JourneysHistoryServices(ApplicationDbContext context) : IJourneysHistoryServices
    {

        public void AddJourney(JourneyDto journeyDto) //wait
        {
            ArgumentNullException.ThrowIfNull(journeyDto);

            context.Journeys.Add(journeyDto.ToModel());
            context.SaveChanges();
        }

        public Task<List<ReturnedHistoryJourneyDto>> GetAllJourneys() => Task.FromResult(context.Journeys
            .Include(j => j.Destination)
            .Include(j => j.StartBusStop)
            .AsEnumerable()
            .Select(j => j.FromJourneyHistoryToReturnedDto())
            .Take(100)
            .ToList());

        public async Task<JourneyHistory> GetJourneyById(Guid id) =>
            await context.Journeys.FirstOrDefaultAsync(j => j.Id.CompareTo(id) == 0) ??
            throw new Exception("Can't find journey with this id");
    }
}
