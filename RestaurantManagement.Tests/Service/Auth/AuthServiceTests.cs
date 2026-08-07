using Moq;
using NUnit.Framework;
using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;
using RestaurantManagement.Exceptions;
using RestaurantManagement.Models;
using RestaurantManagement.Repository.Interfaces;
using RestaurantManagement.Services.Auth;
using System;
using System.Threading.Tasks;

namespace RestaurantManagement.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private Mock<IPasswordHasher> _passwordHasher;
        private Mock<IJwtService> _jwtService;
        private Mock<IRefreshTokenService> _refreshTokenService;

        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _passwordHasher = new Mock<IPasswordHasher>();
            _jwtService = new Mock<IJwtService>();
            _refreshTokenService = new Mock<IRefreshTokenService>();

            _authService = new AuthService(
                _userRepository.Object,
                _refreshTokenRepository.Object,
                _passwordHasher.Object,
                _jwtService.Object,
                _refreshTokenService.Object);
        }
        [Test]
        public void SignupAsync_NullRequest_ThrowsArgumentNullException()
        {
            SignupRequest request = null;

            Func<Task> action = () => _authService.SignupAsync(request);

            Assert.ThrowsAsync<ArgumentNullException>(action);
        }

        [Test]
        public void SignupAsync_EmailAlreadyExists_ThrowsEmailAlreadyExistsException()
        {
            SignupRequest request = new SignupRequest
            {
                Name = "Ritesh",
                Email = "ritesh@test.com",
                Phone = "9999999999",
                Password = "Password@123"
            };

            _userRepository
                .Setup(x => x.EmailExistsAsync(request.Email))
                .ReturnsAsync(true);

            Func<Task> action = () => _authService.SignupAsync(request);

            Assert.ThrowsAsync<EmailAlreadyExistsException>(action);

            _userRepository.Verify(
                x => x.EmailExistsAsync(request.Email),
                Times.Once);

            _userRepository.Verify(
                x => x.PhoneExistsAsync(It.IsAny<string>()),
                Times.Never);

            _userRepository.Verify(
                x => x.Add(It.IsAny<User>()),
                Times.Never);

            _refreshTokenRepository.Verify(
                x => x.Add(It.IsAny<RefreshToken>()),
                Times.Never);

            _jwtService.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);
        }

        [Test]
        public void SignupAsync_PhoneAlreadyExists_ThrowsPhoneAlreadyExistsException()
        {
            SignupRequest request = new SignupRequest
            {
                Name = "Ritesh",
                Email = "ritesh@test.com",
                Phone = "9999999999",
                Password = "Password@123"
            };

            _userRepository
                .Setup(x => x.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            _userRepository
                .Setup(x => x.PhoneExistsAsync(request.Phone))
                .ReturnsAsync(true);

            Func<Task> action = () => _authService.SignupAsync(request);

            Assert.ThrowsAsync<PhoneAlreadyExistsException>(action);

            _userRepository.Verify(
                x => x.EmailExistsAsync(request.Email),
                Times.Once);

            _userRepository.Verify(
                x => x.PhoneExistsAsync(request.Phone),
                Times.Once);

            _userRepository.Verify(
                x => x.Add(It.IsAny<User>()),
                Times.Never);

            _refreshTokenRepository.Verify(
                x => x.Add(It.IsAny<RefreshToken>()),
                Times.Never);

            _jwtService.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Never);
        }


        [Test]
        public async Task SignupAsync_ValidRequest_ReturnsAuthResponse()
        {
            SignupRequest request = new SignupRequest
            {
                Name = "Ritesh",
                Email = "ritesh@test.com",
                Phone = "9999999999",
                Password = "Password@123"
            };

            _userRepository
                .Setup(x => x.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            _userRepository
                .Setup(x => x.PhoneExistsAsync(request.Phone))
                .ReturnsAsync(false);

            _passwordHasher
                .Setup(x => x.HashPassword(request.Password))
                .Returns("hashed-password");

            _userRepository
                .Setup(x => x.Add(It.IsAny<User>()))
                .Callback<User>(u => u.UserId = 1);

            _jwtService
                .Setup(x => x.GenerateAccessToken(It.IsAny<User>()))
                .Returns("access-token");

            RefreshTokenResult refreshTokenResult = new RefreshTokenResult
            {
                PlainTextToken = "refresh-token",
                RefreshTokenEntity = new RefreshToken
                {
                    TokenHash = "hash",
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7)
                }
            };

            _refreshTokenService
                .Setup(x => x.CreateRefreshToken())
                .Returns(refreshTokenResult);

            AuthResponse response = await _authService.SignupAsync(request);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.UserId, Is.EqualTo(1));
            Assert.That(response.Name, Is.EqualTo(request.Name));
            Assert.That(response.Email, Is.EqualTo(request.Email));
            Assert.That(response.AccessToken, Is.EqualTo("access-token"));
            Assert.That(response.RefreshToken, Is.EqualTo("refresh-token"));

            _userRepository.Verify(
                x => x.EmailExistsAsync(request.Email),
                Times.Once);

            _userRepository.Verify(
                x => x.PhoneExistsAsync(request.Phone),
                Times.Once);

            _passwordHasher.Verify(
                x => x.HashPassword(request.Password),
                Times.Once);

            _userRepository.Verify(
                x => x.Add(It.IsAny<User>()),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            _jwtService.Verify(
                x => x.GenerateAccessToken(It.IsAny<User>()),
                Times.Once);

            _refreshTokenService.Verify(
                x => x.CreateRefreshToken(),
                Times.Once);
        }

    }
}

