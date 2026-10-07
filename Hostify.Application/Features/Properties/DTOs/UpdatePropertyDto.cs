using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Application.Features.Properties.DTOs
{
    public class UpdatePropertyDto
    {
        public string Title { get; set; }
        public decimal PricePerNight { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
    }
}
