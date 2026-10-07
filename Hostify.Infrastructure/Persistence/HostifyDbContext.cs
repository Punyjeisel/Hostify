using Microsoft.EntityFrameworkCore;
using Hostify.Domain.Entities;


namespace Hostify.Infrastructure.Persistence
{
    public class HostifyDbContext : DbContext // DbContext es la clase base de Entity Framework Core que representa una sesión con la base de datos y se utiliza para consultar y guardar instancias de tus entidades.
    {
        //Constructor que recibe las opciones de configuración para el DbContext
        public HostifyDbContext(DbContextOptions<HostifyDbContext> options)
            : base(options)
        {
        }

        // Define las propiedades DbSet para cada entidad en tu modelo de dominio 
        public DbSet<User> User => Set<User>();
        public DbSet<Property> properties => Set<Property>();
        public DbSet<Reservation> reservations => Set<Reservation>();
        public DbSet<BlockedDate> blockedDates => Set<BlockedDate>();
        public DbSet<Review> reviews => Set<Review>();
        public DbSet<Notification> notifications => Set<Notification>();
        public DbSet<EmailConfirmationToken> emailConfirmationTokens => Set<EmailConfirmationToken>();
        public DbSet<UserRoleEntity> userRoles => Set<UserRoleEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Este método se llama cuando se crea el modelo de la base de datos y se utiliza para configurar las relaciones entre las tablas, las restricciones, los índices, etc.
        {
            base.OnModelCreating(modelBuilder);
            // configura la relasiones entre las tablas 

            modelBuilder.Entity<Property>()
                .Property(p => p.PricePerNight)
                .HasPrecision(18, 2); // Configura la precisión en Decimas y enteros para el campo PricePerNight

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Host)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.HostId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada para prevenir conflictos de múltiples rutas

            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalPrice)
                .HasPrecision(18, 2); // Configura la precisión en Decimas y enteros para el campo TotalPrice

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Property)
                .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada para prevenir conflictos de múltiples rutas

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Guest)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada para prevenir conflictos de múltiples rutas

            modelBuilder.Entity<UserRoleEntity>()
                .HasKey(ur => new { ur.UserId, ur.Role }); // Configura la clave primaria compuesta para la tabla de roles de usuario

            modelBuilder.Entity<UserRoleEntity>() // Configura la relación entre UserRoleEntity y User
                .HasOne(ur => ur.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.UserId);

        }
    }
}