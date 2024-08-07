﻿using System.ComponentModel.DataAnnotations;

namespace Transportation.Core.Dto.UserInput
{
    public class TicketDto
    {
        [Required]
        public string SeatId { get; set; }
        [Required]
        public string JourneyId { get; set; }
    }
}