using RestaurantManagement.Constants;
using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;
using RestaurantManagement.Enums;
using RestaurantManagement.Exceptions;
using RestaurantManagement.Models;
using RestaurantManagement.Constants;
using RestaurantManagement.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<AuthResponse> SignupAsync(SignupRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new EmailAlreadyExistsException(request.Email);
            }

            if (!string.IsNullOrWhiteSpace(request.Phone) &&
                await _userRepository.PhoneExistsAsync(request.Phone))
            {
                throw new PhoneAlreadyExistsException(request.Phone);
            }

            User user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                RoleId = (int)UserRole.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsActive = true,
                RefreshTokens = new List<RefreshToken>()
            };

            RefreshTokenResult refreshTokenResult = _refreshTokenService.CreateRefreshToken();

            user.RefreshTokens.Add(refreshTokenResult.RefreshTokenEntity);

            _userRepository.Add(user);

            try
            {
                await _userRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                SqlException sqlException = ex.InnerException?.InnerException as SqlException;

                if (sqlException != null &&
                    (sqlException.Number == DatabaseErrorCodes.UniqueConstraintViolation ||
                     sqlException.Number == DatabaseErrorCodes.PrimaryKeyViolation))
                {
                    if (sqlException.Message.Contains("IX_Users_Email"))
                    {
                        throw new EmailAlreadyExistsException(request.Email);
                    }

                    if (sqlException.Message.Contains("IX_Users_Phone"))
                    {
                        throw new PhoneAlreadyExistsException(request.Phone);
                    }
                }

                throw;
            }


            string accessToken = _jwtService.GenerateAccessToken(user);

            return new AuthResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshTokenResult.PlainTextToken
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            User user = await _userRepository.GetByEmailAsync(request.Email);

            if(user == null)
            {
                throw new InvalidCredentialsException();
            }

            if (!user.IsActive)
            {
                throw new InactiveUserException();
            }

            bool isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            if(!isPasswordValid)
            {
                throw new InvalidCredentialsException();
            }

            string accessToken = _jwtService.GenerateAccessToken(user);

            RefreshTokenResult refreshTokenResult = _refreshTokenService.CreateRefreshToken(user.UserId);

            _refreshTokenRepository.Add(refreshTokenResult.RefreshTokenEntity);

            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email= user.Email,
                AccessToken= accessToken,
                RefreshToken= refreshTokenResult.PlainTextToken,
            };

        }
        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new InvalidRefreshTokenException();
            }

            string tokenHash = _refreshTokenService.HashRefreshToken(request.RefreshToken);

            RefreshToken refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            if (refreshToken == null)
            {
                throw new InvalidRefreshTokenException();
            }

            if (refreshToken.IsRevoked)
            {
                List<RefreshToken> userTokens =
                    await _refreshTokenRepository.GetAllByUserIdAsync(refreshToken.UserId);

                foreach (RefreshToken token in userTokens)
                {
                    _refreshTokenService.RevokeRefreshToken(token);
                }

                await _refreshTokenRepository.SaveChangesAsync();

                throw new InvalidRefreshTokenException();
            }

            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new InvalidRefreshTokenException();
            }

            User user = await _userRepository.GetByIdWithRoleAsync(refreshToken.UserId);

            if (user == null)
            {
                throw new InvalidRefreshTokenException();
            }

            _refreshTokenService.RevokeRefreshToken(refreshToken);

            RefreshTokenResult newRefreshTokenResult =
                _refreshTokenService.CreateRefreshToken(user.UserId);

            _refreshTokenRepository.Add(newRefreshTokenResult.RefreshTokenEntity);

            string accessToken = _jwtService.GenerateAccessToken(user);

            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenResult.PlainTextToken
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            string tokenHash = _refreshTokenService.HashRefreshToken(refreshToken);

            RefreshToken token = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            if (token == null)
            {
                return;
            }

            if (token.IsRevoked)
            {
                List<RefreshToken> userTokens = await _refreshTokenRepository.GetAllByUserIdAsync(token.UserId);

                foreach (RefreshToken userToken in userTokens)
                {
                    _refreshTokenService.RevokeRefreshToken(userToken);
                }

                await _refreshTokenRepository.SaveChangesAsync();

                return;
            }

            _refreshTokenService.RevokeRefreshToken(token);

            await _refreshTokenRepository.SaveChangesAsync();
        }
    }
}
