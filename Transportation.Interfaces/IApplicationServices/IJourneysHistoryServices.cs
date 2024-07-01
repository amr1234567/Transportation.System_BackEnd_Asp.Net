using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Dto.ServiceInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Models;

namespace Transportation.Interfaces.IApplicationServices
{
    public interface IJourneysHistoryServices
    {
        void AddJourney(JourneyDto journeyDto);
        Task<List<ReturnedHistoryJourneyDto>> GetAllJourneys();
        Task<JourneyHistory> GetJourneyById(Guid id);
    }
}
