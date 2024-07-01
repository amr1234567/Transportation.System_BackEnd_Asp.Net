using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Helpers.Functions;
using Transportation.Core.Models;
using Transportation.Interfaces.IApplicationServices;

namespace Transportation.Services.ApplicationServices
{
    public class SeatServices(ApplicationDbContext context, IBusServices busServices) : ISeatServices
    {

        public async Task ReserveSeat(Guid id)//done
        {
            var seat = await GetSeatById(id);

            if (!seat.IsAvailable)
                throw new Exception($"Seat with id {id} not available");

            seat.IsAvailable = false;
            context.Seats.Update(seat);
            await context.SaveChangesAsync();
        }

        public async Task SetBusSeatsToAvailable(Guid busId)
        {
            var bus = await busServices.GetBusWithItsSeats(busId);
            foreach (var seat in bus.seats)
            {
                seat.IsAvailable = true;
                context.Seats.Update(seat);
            }
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SeatDto>> GetAllSeatsInBusByBusId(Guid busId) => await context.Seats
            .Where(s => s.BusId.Equals(busId))
            .Select(s => s.FromSeatToSeatDto())
            .ToListAsync();


        private async Task<Seat> GetSeatById(Guid id) =>
            await context.Seats.FirstOrDefaultAsync(s => s.SeatId.Equals(id)) ??
            throw new Exception("Can't find seat with this id");
    }
}
