using Microsoft.IdentityModel.JsonWebTokens;
using Moq;
using NUnit.Framework;
using RestaurantManagement.Controllers;
using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;
using RestaurantManagement.Exceptions;
using RestaurantManagement.Services.Users.Interfaces;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace RestaurantManagement.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userService;
        private UserController _controller;

    [SetUp]
        public void Setup()
        {
            _userService = new Mock<IUserService>();
            _controller = new UserController(_userService.Object);

            ClaimsIdentity identity = new ClaimsIdentity(
                new[]
                {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    "1")
                },
                "TestAuthentication");

            _controller.User = new ClaimsPrincipal(identity);
        }

        [Test]
        public async Task PatchProfile_NullRequest_ReturnsBadRequest()
        {
            PatchProfileRequest request = null;

            IHttpActionResult result =
                await _controller.PatchProfile(request);

            BadRequestErrorMessageResult badRequest =
                result as BadRequestErrorMessageResult;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.Message,
                Is.EqualTo("Request cannot be null."));

            _userService.Verify(
                x => x.PatchProfileAsync(
                    It.IsAny<long>(),
                    It.IsAny<PatchProfileRequest>()),
                Times.Never);
        }

        [Test]
        public async Task PatchProfile_InvalidModelState_ReturnsBadRequest()
        {
            PatchProfileRequest request =
                new PatchProfileRequest();

            _controller.ModelState.AddModelError(
                "Email",
                "Email is invalid.");

            IHttpActionResult result =
                await _controller.PatchProfile(request);

            InvalidModelStateResult badRequest =
                result as InvalidModelStateResult;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.ModelState.IsValid,
                Is.False);

            _userService.Verify(
                x => x.PatchProfileAsync(
                    It.IsAny<long>(),
                    It.IsAny<PatchProfileRequest>()),
                Times.Never);
        }

        [Test]
        public async Task PatchProfile_ValidRequest_ReturnsOk()
        {
            PatchProfileRequest request =
                new PatchProfileRequest();

            UserProfileResponse response =
                new UserProfileResponse
                {
                    UserId = 1,
                    Name = "Ritesh",
                    Email = "ritesh@test.com",
                    Phone = "9999999999"
                };

            _userService
                .Setup(x => x.PatchProfileAsync(
                    1,
                    request))
                .ReturnsAsync(response);

            IHttpActionResult result =
                await _controller.PatchProfile(request);

            OkNegotiatedContentResult<UserProfileResponse> okResult =
                result as OkNegotiatedContentResult<UserProfileResponse>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.UserId, Is.EqualTo(1));
            Assert.That(okResult.Content.Name, Is.EqualTo("Ritesh"));
            Assert.That(
                okResult.Content.Email,
                Is.EqualTo("ritesh@test.com"));
            Assert.That(
                okResult.Content.Phone,
                Is.EqualTo("9999999999"));

            _userService.Verify(
                x => x.PatchProfileAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task PatchProfile_UserNotFound_ReturnsNotFound()
        {
            PatchProfileRequest request =
                new PatchProfileRequest();

            _userService
                .Setup(x => x.PatchProfileAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new UserNotFoundException());

            IHttpActionResult result =
                await _controller.PatchProfile(request);

            NegotiatedContentResult<string> notFound =
                result as NegotiatedContentResult<string>;

            Assert.That(notFound, Is.Not.Null);
            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(
                notFound.Content,
                Is.EqualTo("User not found."));

            _userService.Verify(
                x => x.PatchProfileAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task PatchProfile_EmailAlreadyExists_ReturnsConflict()
        {
            PatchProfileRequest request =
                new PatchProfileRequest();

            _userService
                .Setup(x => x.PatchProfileAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new EmailAlreadyExistsException(
                        "ritesh@test.com"));

            IHttpActionResult result =
                await _controller.PatchProfile(request);

            NegotiatedContentResult<string> conflict =
                result as NegotiatedContentResult<string>;

            Assert.That(conflict, Is.Not.Null);
            Assert.That(
                conflict.StatusCode,
                Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(
                conflict.Content,
                Is.EqualTo(
                    "Email 'ritesh@test.com' is already registered."));

            _userService.Verify(
                x => x.PatchProfileAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task PatchProfile_PhoneAlreadyExists_ReturnsConflict()
        {
            PatchProfileRequest request =
                new PatchProfileRequest();

            _userService
                .Setup(x => x.PatchProfileAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new PhoneAlreadyExistsException(
                        "9999999999"));

            IHttpActionResult result =
                await _controller.PatchProfile(request);

            NegotiatedContentResult<string> conflict =
                result as NegotiatedContentResult<string>;

            Assert.That(conflict, Is.Not.Null);
            Assert.That(
                conflict.StatusCode,
                Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(
                conflict.Content,
                Is.EqualTo(
                    "Phone number '9999999999' is already registered."));

            _userService.Verify(
                x => x.PatchProfileAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task PatchProfile_ServiceThrowsException_ReturnsInternalServerError()
        {
            PatchProfileRequest request =
                new PatchProfileRequest();

            _userService
                .Setup(x => x.PatchProfileAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new Exception("Database error"));

            IHttpActionResult result =
                await _controller.PatchProfile(request);

            ExceptionResult exceptionResult =
                result as ExceptionResult;

            Assert.That(exceptionResult, Is.Not.Null);
            Assert.That(
                exceptionResult.Exception.Message,
                Is.EqualTo("Database error"));

            _userService.Verify(
                x => x.PatchProfileAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task DeactivateAccount_ValidRequest_ReturnsOk()
        {
            _userService
                .Setup(x => x.DeactivateAccountAsync(1))
                .Returns(Task.CompletedTask);

            IHttpActionResult result =
                await _controller.DeactivateAccount();

            OkNegotiatedContentResult<MessageResponse> okResult =
                result as OkNegotiatedContentResult<MessageResponse>;

            Assert.That(okResult, Is.Not.Null);

            Assert.That(
                okResult.Content.Message,
                Is.EqualTo("Account deactivated successfully."));

            _userService.Verify(
                x => x.DeactivateAccountAsync(1),
                Times.Once);
        }

        [Test]
        public async Task DeactivateAccount_UserNotFound_ReturnsNotFound()
        {
            _userService
                .Setup(x => x.DeactivateAccountAsync(1))
                .ThrowsAsync(
                    new UserNotFoundException());

            IHttpActionResult result =
                await _controller.DeactivateAccount();

            NegotiatedContentResult<string> notFound =
                result as NegotiatedContentResult<string>;

            Assert.That(notFound, Is.Not.Null);
            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(
                notFound.Content,
                Is.EqualTo("User not found."));

            _userService.Verify(
                x => x.DeactivateAccountAsync(1),
                Times.Once);
        }

        [Test]
        public async Task DeactivateAccount_ServiceThrowsException_ReturnsInternalServerError()
        {
            _userService
                .Setup(x => x.DeactivateAccountAsync(1))
                .ThrowsAsync(
                    new Exception("Database error"));

            IHttpActionResult result =
                await _controller.DeactivateAccount();

            ExceptionResult exceptionResult =
                result as ExceptionResult;

            Assert.That(exceptionResult, Is.Not.Null);
            Assert.That(
                exceptionResult.Exception.Message,
                Is.EqualTo("Database error"));

            _userService.Verify(
                x => x.DeactivateAccountAsync(1),
                Times.Once);
        }

        [Test]
        public async Task ChangePassword_NullRequest_ReturnsBadRequest()
        {
            ChangePasswordRequest request = null;

            IHttpActionResult result =
                await _controller.ChangePassword(request);

            BadRequestErrorMessageResult badRequest =
                result as BadRequestErrorMessageResult;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.Message,
                Is.EqualTo("Request cannot be null."));

            _userService.Verify(
                x => x.ChangePasswordAsync(
                    It.IsAny<long>(),
                    It.IsAny<ChangePasswordRequest>()),
                Times.Never);
        }

        [Test]
        public async Task ChangePassword_InvalidModelState_ReturnsBadRequest()
        {
            ChangePasswordRequest request =
                new ChangePasswordRequest
                {
                    CurrentPassword = "OldPassword",
                    NewPassword = "NewPassword123",
                    ConfirmNewPassword = "NewPassword123"
                };

            _controller.ModelState.AddModelError(
                "NewPassword",
                "Password is invalid.");

            IHttpActionResult result =
                await _controller.ChangePassword(request);

            InvalidModelStateResult badRequest =
                result as InvalidModelStateResult;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.ModelState.IsValid,
                Is.False);

            _userService.Verify(
                x => x.ChangePasswordAsync(
                    It.IsAny<long>(),
                    It.IsAny<ChangePasswordRequest>()),
                Times.Never);
        }

        [Test]
        public async Task ChangePassword_ValidRequest_ReturnsOk()
        {
            ChangePasswordRequest request =
            new ChangePasswordRequest
            {
                CurrentPassword = "OldPassword123",
                NewPassword = "NewPassword123",
                ConfirmNewPassword = "NewPassword123"
            };

            _userService
                .Setup(x => x.ChangePasswordAsync(
                    1,
                    request))
                .Returns(Task.CompletedTask);

            IHttpActionResult result =
                await _controller.ChangePassword(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(
                result.GetType().Name,
                Is.EqualTo("OkNegotiatedContentResult`1"));

            _userService.Verify(
                x => x.ChangePasswordAsync(
                    1,
                    request),
                Times.Once);

}


        [Test]
        public async Task ChangePassword_UserNotFound_ReturnsNotFound()
        {
            ChangePasswordRequest request =
                new ChangePasswordRequest
                {
                    CurrentPassword = "OldPassword123",
                    NewPassword = "NewPassword123",
                    ConfirmNewPassword = "NewPassword123"
                };

            _userService
                .Setup(x => x.ChangePasswordAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new UserNotFoundException());

            IHttpActionResult result =
                await _controller.ChangePassword(request);

            NegotiatedContentResult<string> notFound =
                result as NegotiatedContentResult<string>;

            Assert.That(notFound, Is.Not.Null);
            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(
                notFound.Content,
                Is.EqualTo("User not found."));

            _userService.Verify(
                x => x.ChangePasswordAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task ChangePassword_InvalidPassword_ReturnsBadRequest()
        {
            ChangePasswordRequest request =
                new ChangePasswordRequest
                {
                    CurrentPassword = "WrongPassword",
                    NewPassword = "NewPassword123",
                    ConfirmNewPassword = "NewPassword123"
                };

            _userService
                .Setup(x => x.ChangePasswordAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new InvalidPasswordException());

            IHttpActionResult result =
                await _controller.ChangePassword(request);

            NegotiatedContentResult<string> badRequest =
                result as NegotiatedContentResult<string>;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(
                badRequest.Content,
                Is.EqualTo("Current password is incorrect."));

            _userService.Verify(
                x => x.ChangePasswordAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task ChangePassword_SamePassword_ReturnsBadRequest()
        {
            ChangePasswordRequest request =
                new ChangePasswordRequest
                {
                    CurrentPassword = "Password123",
                    NewPassword = "Password123",
                    ConfirmNewPassword = "Password123"
                };

            _userService
                .Setup(x => x.ChangePasswordAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new SamePasswordException());

            IHttpActionResult result =
                await _controller.ChangePassword(request);

            NegotiatedContentResult<string> badRequest =
                result as NegotiatedContentResult<string>;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(
                badRequest.Content,
                Is.EqualTo(
                    "New password cannot be the same as current password."));

            _userService.Verify(
                x => x.ChangePasswordAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task ChangePassword_ServiceThrowsException_ReturnsInternalServerError()
        {
            ChangePasswordRequest request =
                new ChangePasswordRequest
                {
                    CurrentPassword = "OldPassword123",
                    NewPassword = "NewPassword123",
                    ConfirmNewPassword = "NewPassword123"
                };

            _userService
                .Setup(x => x.ChangePasswordAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new Exception("Database error"));

            IHttpActionResult result =
                await _controller.ChangePassword(request);

            ExceptionResult exceptionResult =
                result as ExceptionResult;

            Assert.That(exceptionResult, Is.Not.Null);
            Assert.That(
                exceptionResult.Exception.Message,
                Is.EqualTo("Database error"));

            _userService.Verify(
                x => x.ChangePasswordAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task AddAddress_NullRequest_ReturnsBadRequest()
        {
            AddAddressRequest request = null;

            IHttpActionResult result =
                await _controller.AddAddress(request);

            BadRequestErrorMessageResult badRequest =
                result as BadRequestErrorMessageResult;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.Message,
                Is.EqualTo("Request cannot be null."));

            _userService.Verify(
                x => x.AddAddressAsync(
                    It.IsAny<long>(),
                    It.IsAny<AddAddressRequest>()),
                Times.Never);
        }

        [Test]
        public async Task AddAddress_InvalidModelState_ReturnsBadRequest()
        {
            AddAddressRequest request =
                new AddAddressRequest();

            _controller.ModelState.AddModelError(
                "AddressLine1",
                "Address line is required.");

            IHttpActionResult result =
                await _controller.AddAddress(request);

            InvalidModelStateResult badRequest =
                result as InvalidModelStateResult;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.ModelState.IsValid,
                Is.False);

            _userService.Verify(
                x => x.AddAddressAsync(
                    It.IsAny<long>(),
                    It.IsAny<AddAddressRequest>()),
                Times.Never);
        }

        [Test]
        public async Task AddAddress_ValidRequest_ReturnsOk()
        {
            AddAddressRequest request =
                new AddAddressRequest
                {
                    RecipientName = "Ritesh",
                    Phone = "9999999999",
                    AddressLine1 = "123 Main Street",
                    AddressLine2 = "Near Market",
                    City = "Chandigarh",
                    State = "Punjab",
                    PostalCode = "160001",
                    Country = "India",
                    Landmark = "Near Market"
                };

            UserAddressResponse response =
                new UserAddressResponse
                {
                    UserAddressId = 10,
                    RecipientName = "Ritesh",
                    Phone = "9999999999",
                    AddressLine1 = "123 Main Street",
                    AddressLine2 = "Near Market",
                    City = "Chandigarh",
                    State = "Punjab",
                    PostalCode = "160001",
                    Country = "India",
                    Landmark = "Near Market"
                };

            _userService
                .Setup(x => x.AddAddressAsync(
                    1,
                    request))
                .ReturnsAsync(response);

            IHttpActionResult result =
                await _controller.AddAddress(request);

            OkNegotiatedContentResult<UserAddressResponse> okResult =
                result as OkNegotiatedContentResult<UserAddressResponse>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(
                okResult.Content.UserAddressId,
                Is.EqualTo(10));
            Assert.That(
                okResult.Content.RecipientName,
                Is.EqualTo("Ritesh"));
            Assert.That(
                okResult.Content.Phone,
                Is.EqualTo("9999999999"));
            Assert.That(
                okResult.Content.AddressLine1,
                Is.EqualTo("123 Main Street"));
            Assert.That(
                okResult.Content.City,
                Is.EqualTo("Chandigarh"));
            Assert.That(
                okResult.Content.State,
                Is.EqualTo("Punjab"));
            Assert.That(
                okResult.Content.PostalCode,
                Is.EqualTo("160001"));
            Assert.That(
                okResult.Content.Country,
                Is.EqualTo("India"));

            _userService.Verify(
                x => x.AddAddressAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task AddAddress_UserNotFound_ReturnsNotFound()
        {
            AddAddressRequest request =
                new AddAddressRequest();

            _userService
                .Setup(x => x.AddAddressAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new UserNotFoundException());

            IHttpActionResult result =
                await _controller.AddAddress(request);

            NegotiatedContentResult<string> notFound =
                result as NegotiatedContentResult<string>;

            Assert.That(notFound, Is.Not.Null);
            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(
                notFound.Content,
                Is.EqualTo("User not found."));

            _userService.Verify(
                x => x.AddAddressAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task AddAddress_InactiveUser_ReturnsBadRequest()
        {
            AddAddressRequest request =
                new AddAddressRequest();

            _userService
                .Setup(x => x.AddAddressAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new UserInactiveException());

            IHttpActionResult result =
                await _controller.AddAddress(request);

            NegotiatedContentResult<string> badRequest =
                result as NegotiatedContentResult<string>;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(
                badRequest.Content,
                Is.EqualTo("User account is inactive."));

            _userService.Verify(
                x => x.AddAddressAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task AddAddress_ServiceThrowsException_ReturnsInternalServerError()
        {
            AddAddressRequest request =
                new AddAddressRequest();

            _userService
                .Setup(x => x.AddAddressAsync(
                    1,
                    request))
                .ThrowsAsync(
                    new Exception("Database error"));

            IHttpActionResult result =
                await _controller.AddAddress(request);

            ExceptionResult exceptionResult =
                result as ExceptionResult;

            Assert.That(exceptionResult, Is.Not.Null);
            Assert.That(
                exceptionResult.Exception.Message,
                Is.EqualTo("Database error"));

            _userService.Verify(
                x => x.AddAddressAsync(
                    1,
                    request),
                Times.Once);
        }

        [Test]
        public async Task UpdateAddress_NullRequest_ReturnsBadRequest()
        {
            UpdateAddressRequest request = null;

            IHttpActionResult result =
                await _controller.UpdateAddress(
                    10,
                    request);

            BadRequestErrorMessageResult badRequest =
                result as BadRequestErrorMessageResult;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.Message,
                Is.EqualTo("Request cannot be null."));

            _userService.Verify(
                x => x.UpdateAddressAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<UpdateAddressRequest>()),
                Times.Never);
        }

        [Test]
        public async Task UpdateAddress_InvalidModelState_ReturnsBadRequest()
        {
            UpdateAddressRequest request =
                new UpdateAddressRequest();

            _controller.ModelState.AddModelError(
                "Phone",
                "Phone is invalid.");

            IHttpActionResult result =
                await _controller.UpdateAddress(
                    10,
                    request);

            InvalidModelStateResult badRequest =
                result as InvalidModelStateResult;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.ModelState.IsValid,
                Is.False);

            _userService.Verify(
                x => x.UpdateAddressAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<UpdateAddressRequest>()),
                Times.Never);
        }

        [Test]
        public async Task UpdateAddress_ValidRequest_ReturnsOk()
        {
            UpdateAddressRequest request =
                new UpdateAddressRequest
                {
                    RecipientName = "Ritesh",
                    Phone = "9999999999",
                    AddressLine1 = "456 New Street",
                    AddressLine2 = "Apartment 2",
                    City = "Chandigarh",
                    State = "Punjab",
                    PostalCode = "160002",
                    Country = "India",
                    Landmark = "Near Park"
                };

            UserAddressResponse response =
                new UserAddressResponse
                {
                    UserAddressId = 10,
                    RecipientName = "Ritesh",
                    Phone = "9999999999",
                    AddressLine1 = "456 New Street",
                    AddressLine2 = "Apartment 2",
                    City = "Chandigarh",
                    State = "Punjab",
                    PostalCode = "160002",
                    Country = "India",
                    Landmark = "Near Park"
                };

            _userService
                .Setup(x => x.UpdateAddressAsync(
                    1,
                    10,
                    request))
                .ReturnsAsync(response);

            IHttpActionResult result =
                await _controller.UpdateAddress(
                    10,
                    request);

            OkNegotiatedContentResult<UserAddressResponse> okResult =
                result as OkNegotiatedContentResult<UserAddressResponse>;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(
                okResult.Content.UserAddressId,
                Is.EqualTo(10));
            Assert.That(
                okResult.Content.RecipientName,
                Is.EqualTo("Ritesh"));
            Assert.That(
                okResult.Content.Phone,
                Is.EqualTo("9999999999"));
            Assert.That(
                okResult.Content.AddressLine1,
                Is.EqualTo("456 New Street"));
            Assert.That(
                okResult.Content.City,
                Is.EqualTo("Chandigarh"));
            Assert.That(
                okResult.Content.State,
                Is.EqualTo("Punjab"));
            Assert.That(
                okResult.Content.PostalCode,
                Is.EqualTo("160002"));
            Assert.That(
                okResult.Content.Country,
                Is.EqualTo("India"));

            _userService.Verify(
                x => x.UpdateAddressAsync(
                    1,
                    10,
                    request),
                Times.Once);
        }

        [Test]
        public async Task UpdateAddress_UserNotFound_ReturnsNotFound()
        {
            UpdateAddressRequest request =
                new UpdateAddressRequest();

            _userService
                .Setup(x => x.UpdateAddressAsync(
                    1,
                    10,
                    request))
                .ThrowsAsync(
                    new UserNotFoundException());

            IHttpActionResult result =
                await _controller.UpdateAddress(
                    10,
                    request);

            NegotiatedContentResult<string> notFound =
                result as NegotiatedContentResult<string>;

            Assert.That(notFound, Is.Not.Null);
            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(
                notFound.Content,
                Is.EqualTo("User not found."));

            _userService.Verify(
                x => x.UpdateAddressAsync(
                    1,
                    10,
                    request),
                Times.Once);
        }

        [Test]
        public async Task UpdateAddress_InactiveUser_ReturnsBadRequest()
        {
            UpdateAddressRequest request =
                new UpdateAddressRequest();

            _userService
                .Setup(x => x.UpdateAddressAsync(
                    1,
                    10,
                    request))
                .ThrowsAsync(
                    new UserInactiveException());

            IHttpActionResult result =
                await _controller.UpdateAddress(
                    10,
                    request);

            NegotiatedContentResult<string> badRequest =
                result as NegotiatedContentResult<string>;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(
                badRequest.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(
                badRequest.Content,
                Is.EqualTo("User account is inactive."));

            _userService.Verify(
                x => x.UpdateAddressAsync(
                    1,
                    10,
                    request),
                Times.Once);
        }

        [Test]
        public async Task UpdateAddress_AddressNotFound_ReturnsNotFound()
        {
            UpdateAddressRequest request =
                new UpdateAddressRequest();

            _userService
                .Setup(x => x.UpdateAddressAsync(
                    1,
                    10,
                    request))
                .ThrowsAsync(
                    new AddressNotFoundException());

            IHttpActionResult result =
                await _controller.UpdateAddress(
                    10,
                    request);

            NegotiatedContentResult<string> notFound =
                result as NegotiatedContentResult<string>;

            Assert.That(notFound, Is.Not.Null);
            Assert.That(
                notFound.StatusCode,
                Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(
                notFound.Content,
                Is.EqualTo("Address not found."));

            _userService.Verify(
                x => x.UpdateAddressAsync(
                    1,
                    10,
                    request),
                Times.Once);
        }

        [Test]
        public async Task UpdateAddress_ServiceThrowsException_ReturnsInternalServerError()
        {
            UpdateAddressRequest request =
                new UpdateAddressRequest();

            _userService
                .Setup(x => x.UpdateAddressAsync(
                    1,
                    10,
                    request))
                .ThrowsAsync(
                    new Exception("Database error"));

            IHttpActionResult result =
                await _controller.UpdateAddress(
                    10,
                    request);

            ExceptionResult exceptionResult =
                result as ExceptionResult;

            Assert.That(exceptionResult, Is.Not.Null);
            Assert.That(
                exceptionResult.Exception.Message,
                Is.EqualTo("Database error"));

            _userService.Verify(
                x => x.UpdateAddressAsync(
                    1,
                    10,
                    request),
                Times.Once);
        }
    }
}
