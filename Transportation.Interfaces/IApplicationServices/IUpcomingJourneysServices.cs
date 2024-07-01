
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;

namespace Transportation.Interfaces.IApplicationServices
{
    public interface IUpcomingJourneysServices
    {
        Task<UpcomingJourneyDto> AddUpcomingJourney(UpcomingJourneyDto time);
        void TurnUpcomingJourneysIntoHistoryJourneys();
        Task<IEnumerable<ReturnedUpcomingJourneyDto>> GetAllUpcomingJourneys();
        Task<ReturnedUpcomingJourneyDto> GetJourneyById(Guid id);
        Task<IEnumerable<ReturnedUpcomingJourneyDto>> GetAllJourneysByDestinationBusStopId(string id);
        Task<IEnumerable<ReturnedUpcomingJourneyDto>> GetAllJourneysByStartBusStopId(string id);
        Task<IEnumerable<ReturnedUpcomingJourneyDto>> GetNearestJourneysByBusStopsNames(string destinationId, string startBusStopId);
        Task SetArrivalTime(DateTime time, Guid id);
        Task SetLeavingTime(DateTime time, Guid id);
    }
}
