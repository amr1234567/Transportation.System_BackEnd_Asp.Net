using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Transportation.Core.Dto.UserInput;
using Transportation.Core.Dto.UserOutput;
using Transportation.Core.Models;
using Transportation.Interfaces.IApplicationServices;

namespace Transportation.Services.ApplicationServices
{
    public class BusServices(ApplicationDbContext context) : IBusServices
    {

        public async Task AddBus(BusDto busDto) //done
        {
            ArgumentNullException.ThrowIfNull(busDto);
            var busId = Guid.NewGuid();
            var seats = new List<Seat>();

            var bus = new Bus()
            {
                Id = busId,
                seats = seats,
                NumberOfSeats = busDto.NumberOfSeats,
                IsAvailable = true
            };

            for (var i = 1; i <= busDto.NumberOfSeats; i++)
            {
                seats.Add(new Seat()
                {
                    SeatNum = i,
                    IsAvailable = true,
                    SeatId = Guid.NewGuid(),
                    BusId = busId
                });
            }

            await context.Buses.AddAsync(bus);
            await context.SaveChangesAsync();
        }

        public async Task<ResponseModel<Bus>> EditBus(Guid id, BusDto busDto) //done
        {
            ArgumentNullException.ThrowIfNull(busDto);

            var bus = await context.Buses.Include(b => b.seats)
                          .FirstOrDefaultAsync(b => b.Id.Equals(id))
                      ?? throw new Exception("Id Invalid");

            if (bus.NumberOfSeats < busDto.NumberOfSeats)
            {
                var seats = new List<Seat>();

                for (var i = bus.NumberOfSeats + 1; i <= busDto.NumberOfSeats; i++)
                {
                    seats.Add(new Seat()
                    {
                        SeatNum = i,
                        IsAvailable = true,
                        SeatId = Guid.NewGuid(),
                        BusId = id
                    });
                }
                bus.seats = [.. bus.seats, .. seats];
                context.Seats.AddRange(seats);
            }
            else if (bus.NumberOfSeats > busDto.NumberOfSeats)
            {
                var removedSeats = bus.seats.OrderBy(s => s.SeatNum)
                                            .TakeLast(bus.NumberOfSeats - busDto.NumberOfSeats)
                                            .ToList();

                var updatedSeats = bus.seats.ToList();
                foreach (var seat in removedSeats)
                    updatedSeats.Remove(seat);

                bus.seats = [.. updatedSeats];
            }

            bus.NumberOfSeats = busDto.NumberOfSeats;

            context.Buses.Update(bus);
            await context.SaveChangesAsync();

            return new ResponseModel<Bus>
            {
                StatusCode = 200,
                Message = " Done",
                Body = bus
            };
        }

        public async Task<IEnumerable<Bus>> GetAllBuses() => await context.Buses.ToListAsync();
        public async Task<IEnumerable<Bus>> GetAllBusesWithSeats() => await context.Buses.Include(b => b.seats).ToListAsync();

        public async Task<Bus> GetBusById(Guid id) =>
            await context.Buses.FirstOrDefaultAsync(x => x.Id.Equals(id)) ??
            throw new Exception("Can't find bus with this id");

        public async Task<Bus> GetBusWithItsSeats(Guid busId) =>
            await context.Buses.Include(b => b.seats)
                .FirstOrDefaultAsync(x => x.Id.Equals(busId)) ??
            throw new Exception("Can't find bus with this id");

        public async Task SetBusToAvailable(Guid busId)
        {
            var bus = await GetBusById(busId);
            bus.IsAvailable = true;
            context.Buses.Update(bus);
            await context.SaveChangesAsync();
        }

        public async Task SetBusToAvailable(string busId)
        {
            await SetBusToAvailable(Guid.Parse(busId));
        }

        public async Task SetBusToNotAvailable(Guid busId)
        {
            var bus = await GetBusById(busId);
            bus.IsAvailable = false;
            context.Buses.Update(bus);
            await context.SaveChangesAsync();
        }

        public async Task SetBusToNotAvailable(string busId)
        {
            await SetBusToNotAvailable(Guid.Parse(busId));
        }

        public async Task<int> GetNumberOfAvailableSeats(Guid busId)
        {
            var bus = await GetBusWithItsSeats(busId);
            return bus.seats.Count(s => s.IsAvailable);
        }
    }
}