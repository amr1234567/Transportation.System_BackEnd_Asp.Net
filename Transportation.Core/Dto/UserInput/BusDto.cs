
using System.ComponentModel.DataAnnotations;

namespace Transportation.Core.Dto.UserInput
{
    public class BusDto
    {
        [Required]
        [Range(25, 40)]
        public int NumberOfSeats { get; set; }
    }
}
