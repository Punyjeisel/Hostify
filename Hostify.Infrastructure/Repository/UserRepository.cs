using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Aplication.Features.Auth.Interfaces;
using Hostify.Domain.Entities;
using Hostify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hostify.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly HostifyDbContext _context;

        public UserRepository(HostifyDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.User.AddAsync(user);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.User
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.User
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
