using System.Security.Claims;
using System.Text;
using Hostify.API.Middlewares;
using Hostify.Aplication.Features.Auth.Interfaces;
using Hostify.Aplication.Features.Auth.Services;
using Hostify.Aplication.Features.Notifications.Interfaces;
using Hostify.Aplication.Features.Notifications.Services;
using Hostify.Aplication.Features.Properties.Interfaces;
using Hostify.Aplication.Features.Properties.Services;
using Hostify.Aplication.Features.Reservations.Interfaces;
using Hostify.Aplication.Features.Reservations.Services;
using Hostify.Aplication.Features.Reviews.Interfaces;
using Hostify.Aplication.Features.Reviews.Interfaces;
using Hostify.Aplication.Features.Reviews.Services;
using Hostify.Aplication.Interfaces.Security;
using Hostify.Infrastructure.configuration;
using Hostify.Infrastructure.Persistence;
using Hostify.Infrastructure.Repository;
using Hostify.Infrastructure.Security;
using Hostify.Infrastructure.Services.BackgroundServices;
using Hostify.Infrastructure.Services.Correos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace Hostify
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(); // Agrega soporte para controladores de API

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer(); // Agrega el explorador de endpoints para Swagger

            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Agrega tu token JWT aquí"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddDbContext<HostifyDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Configura el DbContext para usar SQL Server con la cadena de conexión del appsettings.json

            builder.Services.AddScoped<IAuthService, AuthService>(); // Agrega el servicio de autenticación

            builder.Services.AddScoped<IEmailService, EmailService>(); // Agrega el servicio de envío de correos electrónicos

            builder.Services.AddScoped<IUserRepository, UserRepository>(); // Agrega CORS para permitir solicitudes desde el cliente React

            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>(); // Agrega el servicio de hashing de contraseñas

            builder.Services.AddScoped<IEmailConfirmationTokenRepository, EmailConfirmationTokenRepository>(); // Agrega el repositorio de tokens de confirmación de correo electrónico

            builder.Services.AddScoped<IJwtSevice, JwtService>(); // Agrega el servicio de generación de JWT

            builder.Services.AddScoped<IPropertyRepository, PropertyRepository>(); // Agrega el repositorio de propiedades

            builder.Services.AddScoped<IPropertyService, PropertyService>(); // Agrega el servicio   de propiedades

            builder.Services.AddScoped<IReservationService, ReservationService>(); // Agrega el servicio de reservas

            builder.Services.AddScoped<IReservationRepository, ReservationRepository>(); // Agrega el repositorio de reservas

            builder.Services.AddHostedService<ReservationBackgroundService>(); // Agrega el servicio en segundo plano para manejar las reservas pendientes

            builder.Services.AddScoped<IReviewService, ReviewService>(); // Agrega el servicio de reseñas

            builder.Services.AddScoped<IReviewRepository, ReviewRepository>(); // Agrega el repositorio de reseñas

            builder.Services.AddScoped<INotificationService , NotificationService>(); // Agrega el servicio de notificaciones

            builder.Services.AddScoped<INotificationRepository, NotificationRepository>(); // Agrega el repositorio de notificaciones

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        RoleClaimType = ClaimTypes.Role // Configura el tipo de claim para los roles, lo que permite que la autorización basada en roles funcione correctamente
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("HostOnly", policy => 
                policy.RequireRole("Host")); // Define una política de autorización que requiere el rol "Host"
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()); // Configura el serializador JSON para convertir los enums a cadenas legibles en lugar de números
                });

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
