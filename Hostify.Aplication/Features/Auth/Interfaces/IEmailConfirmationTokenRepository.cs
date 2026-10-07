using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Domain.Entities;

namespace Hostify.Aplication.Features.Auth.Interfaces
{
    public interface IEmailConfirmationTokenRepository
    {
        Task AddAsync(EmailConfirmationToken token);
        Task<EmailConfirmationToken> GetByTokenAsync(string token);
        Task SaveChangesAsync();
    }
}
