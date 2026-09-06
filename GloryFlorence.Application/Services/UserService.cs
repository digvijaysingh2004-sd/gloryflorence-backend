using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common.Exceptions;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;
using GloryFlorence.Domain.Constants;

namespace GloryFlorence.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public UserService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var userList = await _unitOfWork.Users.FindAsync(
                u => u.Username.ToLower() == loginDto.UsernameOrEmail.ToLower() || 
                     u.Email.ToLower() == loginDto.UsernameOrEmail.ToLower(), 
                cancellationToken);

            var user = userList.FirstOrDefault();

            if (user == null)
            {
                throw new UnauthorizedException("Invalid username/email or password.");
            }

            // Active/inactive user check
            if (!user.IsActive)
            {
                throw new UnauthorizedException("User account is inactive. Please contact your administrator.");
            }

            // Password verification
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedException("Invalid username/email or password.");
            }

            var token = _tokenService.GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                User = MapToDto(user)
            };
        }

        public async Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), id);
            }
            return MapToDto(user);
        }

        public async Task<UserDto> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var userList = await _unitOfWork.Users.FindAsync(
                u => u.Username.ToLower() == username.ToLower(), 
                cancellationToken);
            var user = userList.FirstOrDefault();
            if (user == null)
            {
                throw new NotFoundException(nameof(User), username);
            }
            return MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            // Check for duplicate username
            var existingUsername = await _unitOfWork.Users.FindAsync(
                u => u.Username.ToLower() == createUserDto.Username.ToLower(), 
                cancellationToken);
            if (existingUsername.Any())
            {
                throw new BadRequestException($"Username '{createUserDto.Username}' is already taken.");
            }

            // Check for duplicate email
            var existingEmail = await _unitOfWork.Users.FindAsync(
                u => u.Email.ToLower() == createUserDto.Email.ToLower(), 
                cancellationToken);
            if (existingEmail.Any())
            {
                throw new BadRequestException($"Email '{createUserDto.Email}' is already registered.");
            }

            // Role claims validation
            if (!Roles.All.Contains(createUserDto.Role))
            {
                throw new BadRequestException($"Invalid role '{createUserDto.Role}'. Valid roles: {string.Join(", ", Roles.All)}");
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                Role = createUserDto.Role,
                IsActive = true,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(user);
        }

        public async Task UpdateUserAsync(UpdateUserDto updateUserDto, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(updateUserDto.Id, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), updateUserDto.Id);
            }

            // Check for duplicate email excluding current user
            var existingEmail = await _unitOfWork.Users.FindAsync(
                u => u.Email.ToLower() == updateUserDto.Email.ToLower() && u.Id != updateUserDto.Id, 
                cancellationToken);
            if (existingEmail.Any())
            {
                throw new BadRequestException($"Email '{updateUserDto.Email}' is already registered.");
            }

            // Role claims validation
            if (!Roles.All.Contains(updateUserDto.Role))
            {
                throw new BadRequestException($"Invalid role '{updateUserDto.Role}'. Valid roles: {string.Join(", ", Roles.All)}");
            }

            user.Email = updateUserDto.Email;
            user.Role = updateUserDto.Role;
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.IsActive = updateUserDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
            return users.Select(MapToDto);
        }

        public async Task DeleteUserAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), id);
            }

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
