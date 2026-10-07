using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Infrastructure.Persistence;
using Hostify.Infrastructure.Repository;
using Hostify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Hostify.Application.Features.Auth.Interfaces;

namespace Hostify.Infrastructure.Repository
{
    public class EmailConfirmationTokenRepository : IEmailConfirmationTokenRepository
    {
        private readonly HostifyDbContext _context;

        public EmailConfirmationTokenRepository(HostifyDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailConfirmationToken token)
        {
            await _context.emailConfirmationTokens.AddAsync(token);
        }

        public Task<EmailConfirmationToken> GetByTokenAsync(string token)
        {
            return _context.emailConfirmationTokens.FirstOrDefaultAsync(t => t.Token == token);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
