using Moq;
using NUnit.Framework;
using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;
using RestaurantManagement.Exceptions;
using RestaurantManagement.Models;
using RestaurantManagement.Repository.Interfaces;
using RestaurantManagement.Services.Auth;
using RestaurantManagement.Services.Users;
using System;
using System.Threading.Tasks;

namespace RestaurantManagement.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordHasher> _passwordHasher;

        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordHasher = new Mock<IPasswordHasher>();

            _userService = new UserService(
                _userRepository.Object,
                _passwordHasher.Object);
        }

        [Test]
        public void PatchProfileAsync_NullRequest_ThrowsArgumentNullException()
        {
            PatchProfileRequest request = null;

            Func<Task> action = () =>
                _userService.PatchProfileAsync(1, request);

            Assert.ThrowsAsync<ArgumentNullException>(action);

            _userRepository.Verify(
                x => x.GetByIdAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Test]
        public void PatchProfileAsync_NoFieldsProvided_ThrowsArgumentException()
        {
            PatchProfileRequest request = new PatchProfileRequest();

            Func<Task> action = () =>
                _userService.PatchProfileAsync(1, request);

            Assert.ThrowsAsync<ArgumentException>(action);

            _userRepository.Verify(
                x => x.GetByIdAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Test]
        public void PatchProfileAsync_UserNotFound_ThrowsUserNotFoundException()
        {
            PatchProfileRequest request = new PatchProfileRequest
            {
                Name = "Updated Name"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            Func<Task> action = () =>
                _userService.PatchProfileAsync(1, request);

            Assert.ThrowsAsync<UserNotFoundException>(action);

            _userRepository.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _userRepository.Verify(
                x => x.Update(It.IsAny<User>()),
                Times.Never);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Test]
        public void PatchProfileAsync_EmailAlreadyExists_ThrowsEmailAlreadyExistsException()
        {
            PatchProfileRequest request = new PatchProfileRequest
            {
                Email = "new@test.com"
            };

            User user = new User
            {
                UserId = 1,
                Email = "old@test.com"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepository
                .Setup(x => x.EmailExistsAsync(request.Email))
                .ReturnsAsync(true);

            Func<Task> action = () =>
                _userService.PatchProfileAsync(1, request);

            Assert.ThrowsAsync<EmailAlreadyExistsException>(action);

            _userRepository.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _userRepository.Verify(
                x => x.EmailExistsAsync(request.Email),
                Times.Once);

            _userRepository.Verify(
                x => x.Update(It.IsAny<User>()),
                Times.Never);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Test]
        public void PatchProfileAsync_PhoneAlreadyExists_ThrowsPhoneAlreadyExistsException()
        {
            PatchProfileRequest request = new PatchProfileRequest
            {
                Phone = "9999999999"
            };

            User user = new User
            {
                UserId = 1,
                Phone = "8888888888"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepository
                .Setup(x => x.PhoneExistsAsync(request.Phone))
                .ReturnsAsync(true);

            Func<Task> action = () =>
                _userService.PatchProfileAsync(1, request);

            Assert.ThrowsAsync<PhoneAlreadyExistsException>(action);

            _userRepository.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _userRepository.Verify(
                x => x.PhoneExistsAsync(request.Phone),
                Times.Once);

            _userRepository.Verify(
                x => x.Update(It.IsAny<User>()),
                Times.Never);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Test]
        public async Task PatchProfileAsync_ValidNameUpdate_UpdatesUserSuccessfully()
        {
            PatchProfileRequest request = new PatchProfileRequest
            {
                Name = "Updated Name"
            };

            User user = new User
            {
                UserId = 1,
                Name = "Old Name",
                Email = "test@test.com"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            UserProfileResponse response =
                await _userService.PatchProfileAsync(1, request);

            Assert.That(
                response.Name,
                Is.EqualTo("Updated Name"));

            Assert.That(
                response.Email,
                Is.EqualTo("test@test.com"));

            _userRepository.Verify(
                x => x.Update(user),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public async Task PatchProfileAsync_ValidEmailUpdate_UpdatesUserSuccessfully()
        {
            PatchProfileRequest request = new PatchProfileRequest
            {
                Email = "new@test.com"
            };

            User user = new User
            {
                UserId = 1,
                Email = "old@test.com",
                Name = "Ritesh"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepository
                .Setup(x => x.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            UserProfileResponse response =
                await _userService.PatchProfileAsync(1, request);

            Assert.That(
                response.Email,
                Is.EqualTo("new@test.com"));

            Assert.That(
                response.Name,
                Is.EqualTo("Ritesh"));

            _userRepository.Verify(
                x => x.EmailExistsAsync(request.Email),
                Times.Once);

            _userRepository.Verify(
                x => x.Update(user),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public async Task PatchProfileAsync_ValidPhoneUpdate_UpdatesUserSuccessfully()
        {
            PatchProfileRequest request = new PatchProfileRequest
            {
                Phone = "9999999999"
            };

            User user = new User
            {
                UserId = 1,
                Phone = "8888888888",
                Name = "Ritesh",
                Email = "test@test.com"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepository
                .Setup(x => x.PhoneExistsAsync(request.Phone))
                .ReturnsAsync(false);

            UserProfileResponse response =
                await _userService.PatchProfileAsync(1, request);

            Assert.That(
                response.Phone,
                Is.EqualTo("9999999999"));

            Assert.That(
                response.Email,
                Is.EqualTo("test@test.com"));

            _userRepository.Verify(
                x => x.PhoneExistsAsync(request.Phone),
                Times.Once);

            _userRepository.Verify(
                x => x.Update(user),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public async Task PatchProfileAsync_MultipleFieldsUpdate_UpdatesSuccessfully()
        {
            PatchProfileRequest request = new PatchProfileRequest
            {
                Name = "New Name",
                Email = "new@test.com",
                Phone = "9999999999"
            };

            User user = new User
            {
                UserId = 1,
                Name = "Old Name",
                Email = "old@test.com",
                Phone = "8888888888"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepository
                .Setup(x => x.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            _userRepository
                .Setup(x => x.PhoneExistsAsync(request.Phone))
                .ReturnsAsync(false);

            UserProfileResponse response =
                await _userService.PatchProfileAsync(1, request);

            Assert.That(
                response.Name,
                Is.EqualTo("New Name"));

            Assert.That(
                response.Email,
                Is.EqualTo("new@test.com"));

            Assert.That(
                response.Phone,
                Is.EqualTo("9999999999"));

            _userRepository.Verify(
                x => x.EmailExistsAsync(request.Email),
                Times.Once);

            _userRepository.Verify(
                x => x.PhoneExistsAsync(request.Phone),
                Times.Once);

            _userRepository.Verify(
                x => x.Update(user),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public async Task PatchProfileAsync_ValidRequest_CallsUpdateAndSaveChanges()
        {
            PatchProfileRequest request = new PatchProfileRequest
            {
                Name = "Updated Name"
            };

            User user = new User
            {
                UserId = 1,
                Name = "Old Name",
                Email = "test@test.com"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            await _userService.PatchProfileAsync(1, request);

            _userRepository.Verify(
                x => x.Update(user),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public void ChangePasswordAsync_NullRequest_ThrowsArgumentNullException()
        {
            ChangePasswordRequest request = null;

            Func<Task> action = () =>
                _userService.ChangePasswordAsync(1, request);

            Assert.ThrowsAsync<ArgumentNullException>(action);

            _userRepository.Verify(
                x => x.GetByIdAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Test]
        public void ChangePasswordAsync_UserNotFound_ThrowsUserNotFoundException()
        {
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword@123",
                NewPassword = "NewPassword@123"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            Func<Task> action = () =>
                _userService.ChangePasswordAsync(1, request);

            Assert.ThrowsAsync<UserNotFoundException>(action);

            _userRepository.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _passwordHasher.Verify(
                x => x.VerifyPassword(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);

            _userRepository.Verify(
                x => x.Update(It.IsAny<User>()),
                Times.Never);
        }

        [Test]
        public void ChangePasswordAsync_InvalidCurrentPassword_ThrowsInvalidPasswordException()
        {
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                CurrentPassword = "WrongPassword@123",
                NewPassword = "NewPassword@123"
            };

            User user = new User
            {
                UserId = 1,
                PasswordHash = "hashed-password"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(x => x.VerifyPassword(
                    request.CurrentPassword,
                    user.PasswordHash))
                .Returns(false);

            Func<Task> action = () =>
                _userService.ChangePasswordAsync(1, request);

            Assert.ThrowsAsync<InvalidPasswordException>(action);

            _passwordHasher.Verify(
                x => x.VerifyPassword(
                    request.CurrentPassword,
                    user.PasswordHash),
                Times.Once);

            _passwordHasher.Verify(
                x => x.HashPassword(It.IsAny<string>()),
                Times.Never);

            _userRepository.Verify(
                x => x.Update(It.IsAny<User>()),
                Times.Never);
        }

        [Test]
        public void ChangePasswordAsync_SamePassword_ThrowsSamePasswordException()
        {
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                CurrentPassword = "Password@123",
                NewPassword = "Password@123"
            };

            User user = new User
            {
                UserId = 1,
                PasswordHash = "hashed-password"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(x => x.VerifyPassword(
                    request.CurrentPassword,
                    user.PasswordHash))
                .Returns(true);

            Func<Task> action = () =>
                _userService.ChangePasswordAsync(1, request);

            Assert.ThrowsAsync<SamePasswordException>(action);

            _passwordHasher.Verify(
                x => x.HashPassword(It.IsAny<string>()),
                Times.Never);

            _userRepository.Verify(
                x => x.Update(It.IsAny<User>()),
                Times.Never);
        }

        [Test]
        public async Task ChangePasswordAsync_ValidPassword_UpdatesPasswordSuccessfully()
        {
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword@123",
                NewPassword = "NewPassword@123"
            };

            User user = new User
            {
                UserId = 1,
                PasswordHash = "old-hash"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(x => x.VerifyPassword(
                    request.CurrentPassword,
                    user.PasswordHash))
                .Returns(true);

            _passwordHasher
                .Setup(x => x.HashPassword(request.NewPassword))
                .Returns("new-hash");

            await _userService.ChangePasswordAsync(1, request);

            Assert.That(
                user.PasswordHash,
                Is.EqualTo("new-hash"));

            _userRepository.Verify(
                x => x.Update(user),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public async Task ChangePasswordAsync_ValidPassword_CallsHashPassword()
        {
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword@123",
                NewPassword = "NewPassword@123"
            };

            User user = new User
            {
                UserId = 1,
                PasswordHash = "old-hash"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(x => x.VerifyPassword(
                    request.CurrentPassword,
                    user.PasswordHash))
                .Returns(true);

            _passwordHasher
                .Setup(x => x.HashPassword(request.NewPassword))
                .Returns("new-hash");

            await _userService.ChangePasswordAsync(1, request);

            _passwordHasher.Verify(
                x => x.HashPassword(request.NewPassword),
                Times.Once);
        }

        [Test]
        public async Task ChangePasswordAsync_ValidPassword_CallsUpdateAndSaveChanges()
        {
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword@123",
                NewPassword = "NewPassword@123"
            };

            User user = new User
            {
                UserId = 1,
                PasswordHash = "old-hash"
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(x => x.VerifyPassword(
                    request.CurrentPassword,
                    user.PasswordHash))
                .Returns(true);

            _passwordHasher
                .Setup(x => x.HashPassword(request.NewPassword))
                .Returns("new-hash");

            await _userService.ChangePasswordAsync(1, request);

            _userRepository.Verify(
                x => x.Update(user),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public void DeactivateAccountAsync_UserNotFound_ThrowsUserNotFoundException()
        {
            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((User)null);

            Func<Task> action = () =>
                _userService.DeactivateAccountAsync(1);

            Assert.ThrowsAsync<UserNotFoundException>(action);

            _userRepository.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _userRepository.Verify(
                x => x.Update(It.IsAny<User>()),
                Times.Never);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Test]
        public async Task DeactivateAccountAsync_AlreadyInactive_ReturnsWithoutUpdating()
        {
            User user = new User
            {
                UserId = 1,
                IsActive = false
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            await _userService.DeactivateAccountAsync(1);

            Assert.That(
                user.IsActive,
                Is.False);

            _userRepository.Verify(
                x => x.Update(It.IsAny<User>()),
                Times.Never);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Test]
        public async Task DeactivateAccountAsync_ActiveUser_DeactivatesSuccessfully()
        {
            User user = new User
            {
                UserId = 1,
                IsActive = true,
                UpdatedAt = DateTime.MinValue
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            await _userService.DeactivateAccountAsync(1);

            Assert.That(
                user.IsActive,
                Is.False);

            Assert.That(
                user.UpdatedAt,
                Is.GreaterThan(DateTime.MinValue));

            _userRepository.Verify(
                x => x.Update(user),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Test]
        public async Task DeactivateAccountAsync_ActiveUser_CallsUpdateAndSaveChanges()
        {
            User user = new User
            {
                UserId = 1,
                IsActive = true
            };

            _userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            await _userService.DeactivateAccountAsync(1);

            _userRepository.Verify(
                x => x.Update(user),
                Times.Once);

            _userRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }
    }
}
