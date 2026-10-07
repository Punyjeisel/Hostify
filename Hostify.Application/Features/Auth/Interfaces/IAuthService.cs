using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Application.Features.Auth.DTOs;

namespace Hostify.Application.Features.Auth.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task ConfirmEmailAsync(string token);
    }
}
