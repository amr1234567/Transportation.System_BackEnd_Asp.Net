
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Dto.UserOutput;

namespace Transportation.Interfaces.IApplicationServices
{
    public interface ISeatServices
    {
        Task<IEnumerable<SeatDto>> GetAllSeatsInBusByBusId(Guid busId);
        Task ReserveSeat(Guid id);
        Task SetBusSeatsToAvailable(Guid busId);
    }
}
