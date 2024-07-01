using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Models;

namespace Transportation.Core.Identity
{
    public class BusStopManger : User
    {

        public IEnumerable<BusStopManger>? BusStopsRelations { get; set; }
        public IEnumerable<BusStopManger>? BusStops { get; set; }

        public IEnumerable<Bus> Buses { get; set; }
    }
}
