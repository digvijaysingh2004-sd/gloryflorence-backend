using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IUserService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
        Task<LoginResponseDto> RegisterPatientAsync(RegisterPatientDto registerDto, CancellationToken cancellationToken);
        Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserDto> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken);
        Task UpdateUserAsync(UpdateUserDto updateUserDto, CancellationToken cancellationToken);
        Task<UserDto> UpdateProfileAsync(string username, UpdateUserProfileDto dto, CancellationToken cancellationToken);
        Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task DeleteUserAsync(int id, CancellationToken cancellationToken);
    }
}
