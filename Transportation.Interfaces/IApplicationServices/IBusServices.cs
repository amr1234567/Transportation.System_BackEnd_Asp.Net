using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Models;

namespace Transportation.Interfaces.IApplicationServices
{
    public interface IBusServices
    {
        Task AddBus(BusDto busDto);
        Task<IEnumerable<Bus>> GetAllBusesWithSeats();
        Task<IEnumerable<Bus>> GetAllBuses();
        Task<Bus> GetBusById(Guid id);
        Task<ResponseModel<Bus>> EditBus(Guid id, BusDto busDto);
        Task<Bus> GetBusWithItsSeats(Guid busId);
        Task SetBusToAvailable(Guid busId);
        Task SetBusToAvailable(string busId);
        Task SetBusToNotAvailable(Guid busId);
        Task SetBusToNotAvailable(string busId);
        Task<int> GetNumberOfAvailableSeats(Guid busId);
    }
}
