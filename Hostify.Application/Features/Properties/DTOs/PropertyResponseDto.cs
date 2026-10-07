using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Application.Features.Properties.DTOs
{
    public class PropertyResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public Guid HostId { get; set; }
    }
}
