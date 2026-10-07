using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hostify.Application.Features.Auth.DTOs;
using Hostify.Application.Features.Auth.Interfaces;
using Hostify.Application.Interfaces.Security;
using Hostify.Domain.Entities;
using Hostify.Domain.Enums;



namespace Hostify.Application.Features.Auth.Services
{
    public class AuthService : IAuthService // Esta clase implementa la interfaz IAuthService y se encarga de manejar la lógica de autenticación, incluyendo el registro de usuarios y el envío de correos electrónicos de bienvenida.
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailConfirmationTokenRepository _tokenRepository;
        private readonly IJwtService _jwtService;
        

        public AuthService(IUserRepository userRepository, IEmailService emailService, IPasswordHasher passwordHasher, IEmailConfirmationTokenRepository tokenService, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _jwtService = jwtService;
            _tokenRepository = tokenService;
            _passwordHasher = passwordHasher;
        }

        // El método ConfirmEmailAsync es un método que se espera que maneje la lógica para confirmar el correo electrónico de un usuario utilizando un token.
        public async Task ConfirmEmailAsync(string token) 
        {
            var tokenEntity = await _tokenRepository.GetByTokenAsync(token);
            if (tokenEntity == null)
                throw new Exception("Token no válido.");

            if (tokenEntity.UsedAt != null)
                throw new Exception("Token ya utilizado.");

            if (tokenEntity.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Token expirado.");

            var user = await _userRepository.GetByIdAsync(tokenEntity.UserId); // Busca el usuario asociado al token utilizando el UserId almacenado en el tokenEntity. Si no se encuentra el usuario, se lanza una excepción indicando que el usuario no fue encontrado.

            if (user == null)
                throw new Exception("Usuario no encontrado.");

            user.IsEmailConfirmed = true;

            tokenEntity.UsedAt = DateTime.UtcNow; // Marca el token como utilizado estableciendo la propiedad UsedAt con la fecha y hora actual.

            await _userRepository.SaveChangesAsync();
        }


        // El método LoginAsync es un método que se espera que maneje la lógica de inicio de sesión de un usuario utilizando las credenciales proporcionadas en el objeto LoginDto.
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // Busca usuario
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
                throw new Exception("Credenciales inválidas.");

            // Verifica contraseña
            var isValidPassword = _passwordHasher.Verify(dto.Password, user.passwordHash);

            if (!isValidPassword)
                throw new Exception("Credenciales inválidas.");

            // Verifica si el correo electrónico está confirmado
            if (!user.IsEmailConfirmed)
                throw new Exception("Por favor, confirma tu correo electrónico antes de iniciar sesión.");
            
            var token = _jwtService.GenerateToken(user); // Genera un token JWT para el usuario autenticado utilizando el servicio de JWT inyectado. El token se genera a partir de la información del usuario, como su ID, correo electrónico y nombre completo.

            // Devuelve respuesta con token y datos del usuario
            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName
            };
        }


        // El método RegisterAsync es un método que maneja la lógica de registro de un nuevo usuario utilizando los datos proporcionados en el objeto RegisterDto. Actualmente, este método simula el proceso de registro imprimiendo un mensaje en la consola y luego envía un correo electrónico de bienvenida al usuario utilizando el servicio de correo electrónico inyectado.
        public async Task RegisterAsync(RegisterDto dto)
        {
            var ExistingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (ExistingUser != null)
            {
                throw new Exception("El correo electrónico ya está registrado.");
            }

            var hashedPassword = _passwordHasher.Hash(dto.Password);

            // Crea nuevo usuario

            var user = new Hostify.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                passwordHash = hashedPassword,
                IsEmailConfirmed = false,
                CreateAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            // Crea token de confirmación de correo electrónico

            var token = new Hostify.Domain.Entities.EmailConfirmationToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            await _tokenRepository.AddAsync(token);

            // Asigna rol de invitado al nuevo usuario

            user.Roles = new List<UserRoleEntity>
            {
                new UserRoleEntity      
                {
                    Role = UserRole.Guest
                }
            };

            // Guarda el usuario en la base de datos

            await _userRepository.SaveChangesAsync();

            // link de confirmación de correo electrónico (simulado)
            var confirmationLink = $"http://localhost:5056/api/auth/confirm-email?token={token.Token}";

            await _emailService.SendEmailAsync(
                dto.Email,
                "Confirma tu cuenta en Hostify",
                $"Gracias por registrarte en Hostify. ¡Estamos emocionados de tenerte a bordo! Por favor, confirma tu cuenta haciendo clic en el siguiente enlace: {confirmationLink}"
                );
        }
    }
}
