using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Entities;

namespace Hostify.Aplication.Interfaces.Security
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
