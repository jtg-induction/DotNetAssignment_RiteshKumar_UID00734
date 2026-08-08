using RestaurantManagement.Constants;
using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;
using RestaurantManagement.Exceptions;
using RestaurantManagement.Models;
using RestaurantManagement.Repository.Interfaces;
using RestaurantManagement.Services.Auth;
using RestaurantManagement.Services.Users.Interfaces;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;


        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }


        public async Task<UserProfileResponse> PatchProfileAsync(
            long userId,
            PatchProfileRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }


            if (string.IsNullOrWhiteSpace(request.Name)
                && string.IsNullOrWhiteSpace(request.Email)
                && string.IsNullOrWhiteSpace(request.Phone))
            {
                throw new ArgumentException(
                    "At least one field is required for update.");
            }


            User user = await _userRepository.GetByIdAsync(userId);


            if (user == null)
            {
                throw new UserNotFoundException();
            }


            bool emailChanged =
                !string.IsNullOrWhiteSpace(request.Email)
                &&
                !string.Equals(
                    user.Email,
                    request.Email,
                    StringComparison.OrdinalIgnoreCase);


            if (emailChanged)
            {
                bool emailExists =
                    await _userRepository.EmailExistsAsync(request.Email);


                if (emailExists)
                {
                    throw new EmailAlreadyExistsException(request.Email);
                }


                user.Email = request.Email;
            }


            bool phoneChanged =
                request.Phone != null
                &&
                user.Phone != request.Phone;


            if (phoneChanged)
            {
                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    bool phoneExists =
                        await _userRepository.PhoneExistsAsync(request.Phone);


                    if (phoneExists)
                    {
                        throw new PhoneAlreadyExistsException(request.Phone);
                    }
                }


                user.Phone = request.Phone;
            }


            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                user.Name = request.Name;
            }


            user.UpdatedAt = DateTime.UtcNow;


            _userRepository.Update(user);


            try
            {
                await _userRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                SqlException sqlException =
                    ex.InnerException?.InnerException as SqlException;


                if (sqlException != null &&
                    (sqlException.Number == DatabaseErrorCodes.UniqueConstraintViolation
                     ||
                     sqlException.Number == DatabaseErrorCodes.PrimaryKeyViolation))
                {
                    if (sqlException.Message.Contains("IX_User_Email"))
                    {
                        throw new EmailAlreadyExistsException(
                            request.Email);
                    }


                    if (sqlException.Message.Contains("IX_User_Phone"))
                    {
                        throw new PhoneAlreadyExistsException(
                            request.Phone);
                    }
                }


                throw;
            }


            return new UserProfileResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone
            };
        }


        public async Task ChangePasswordAsync(
            long userId,
            ChangePasswordRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }


            User user = await _userRepository.GetByIdAsync(userId);


            if (user == null)
            {
                throw new UserNotFoundException();
            }


            bool isCurrentPasswordValid =
                _passwordHasher.VerifyPassword(
                    request.CurrentPassword,
                    user.PasswordHash);


            if (!isCurrentPasswordValid)
            {
                throw new InvalidPasswordException();
            }


            if (request.CurrentPassword == request.NewPassword)
            {
                throw new SamePasswordException();
            }


            user.PasswordHash =
                _passwordHasher.HashPassword(request.NewPassword);


            user.UpdatedAt = DateTime.UtcNow;


            _userRepository.Update(user);


            await _userRepository.SaveChangesAsync();
        }


        public async Task DeactivateAccountAsync(
            long userId)
        {
            User user = await _userRepository.GetByIdAsync(userId);


            if (user == null)
            {
                throw new UserNotFoundException();
            }


            if (!user.IsActive)
            {
                return;
            }


            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;


            _userRepository.Update(user);


            await _userRepository.SaveChangesAsync();
        }
    }
}
