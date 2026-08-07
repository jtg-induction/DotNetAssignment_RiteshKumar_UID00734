using Moq;
using NUnit.Framework;
using RestaurantManagement.Controllers;
using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;
using RestaurantManagement.Exceptions;
using RestaurantManagement.Services.Auth;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace RestaurantManagement.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authService;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _authService = new Mock<IAuthService>();
            _controller = new AuthController(_authService.Object);
        }

        [Test]
        public async Task Signup_NullRequest_ReturnsBadRequest()
        {
            SignupRequest request = null;
            IHttpActionResult result = await _controller.Signup(request);
            BadRequestErrorMessageResult badRequest = result as BadRequestErrorMessageResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest.Message, Is.EqualTo("Request cannot be null."));
        }

        [Test]
        public async Task Signup_InvalidModelState_ReturnsBadRequest()
        {
            SignupRequest request = new SignupRequest();

            _controller.ModelState.AddModelError(
                "Email",
                "Email is required");
            IHttpActionResult result = await _controller.Signup(request);
            InvalidModelStateResult badRequest = result as InvalidModelStateResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest.ModelState.IsValid, Is.False);

            _authService.Verify(
                x => x.SignupAsync(It.IsAny<SignupRequest>()),
                Times.Never);
        }

        [Test]
        public async Task Signup_ValidRequest_ReturnsOkResult()
        {
            SignupRequest request = new SignupRequest
            {
                Name = "Ritesh",
                Email = "ritesh@test.com",
                Phone = "9999999999",
                Password = "Password@123"
            };

            AuthResponse response = new AuthResponse
            {
                UserId = 1,
                Name = "Ritesh",
                Email = "ritesh@test.com",
                AccessToken = "jwt-token",
                RefreshToken = "refresh-token"
            };

            _authService
                .Setup(x => x.SignupAsync(It.IsAny<SignupRequest>()))
                .ReturnsAsync(response);

            IHttpActionResult result = await _controller.Signup(request);
            OkNegotiatedContentResult<AuthResponse> okResult = result as OkNegotiatedContentResult<AuthResponse>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Content.UserId, Is.EqualTo(1));
            Assert.That(okResult.Content.Email,Is.EqualTo("ritesh@test.com"));

            _authService.Verify(
                x => x.SignupAsync(request),
                Times.Once);
        }

        [Test]
        public async Task Signup_EmailAlreadyExists_ReturnsConflict()
        {
            SignupRequest request = new SignupRequest
            {
                Name = "Ritesh",
                Email = "ritesh@test.com",
                Phone = "9999999999",
                Password = "Password@123"
            };

            _authService
                .Setup(x => x.SignupAsync(It.IsAny<SignupRequest>()))
                .ThrowsAsync(new EmailAlreadyExistsException(request.Email));

            IHttpActionResult result = await _controller.Signup(request);
            NegotiatedContentResult<string> conflict = result as NegotiatedContentResult<string>;
            Assert.That(conflict, Is.Not.Null);
            Assert.That(conflict.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(conflict.Content, Is.EqualTo($"Email '{request.Email}' is already registered."));
            _authService.Verify(
                x => x.SignupAsync(request),
                Times.Once);
        }

        [Test]
        public async Task Signup_PhoneAlreadyExists_ReturnsConflict()
        {
            SignupRequest request = new SignupRequest
            {
                Name = "Ritesh",
                Email = "ritesh@test.com",
                Phone = "9999999999",
                Password = "Password@123"
            };

            _authService
                .Setup(x => x.SignupAsync(It.IsAny<SignupRequest>()))
                .ThrowsAsync(new PhoneAlreadyExistsException(request.Phone));
            IHttpActionResult result = await _controller.Signup(request);
            NegotiatedContentResult<string> conflict = result as NegotiatedContentResult<string>;
            Assert.That(conflict, Is.Not.Null);
            Assert.That(conflict.StatusCode,Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(conflict.Content,Is.EqualTo($"Phone number '{request.Phone}' is already registered."));

            _authService.Verify(
                x => x.SignupAsync(request),
                Times.Once);
        }

        [Test]
        public async Task Signup_ServiceThrowsException_ReturnsInternalServerError()
        {
            SignupRequest request = new SignupRequest
            {
                Name = "Ritesh",
                Email = "ritesh@test.com",
                Phone = "9999999999",
                Password = "Password@123"
            };

            _authService
                .Setup(x => x.SignupAsync(It.IsAny<SignupRequest>()))
                .ThrowsAsync(
                    new Exception("Something went wrong"));
            IHttpActionResult result = await _controller.Signup(request);
            ExceptionResult exceptionResult = result as ExceptionResult;
            Assert.That(exceptionResult, Is.Not.Null);
            Assert.That(exceptionResult.Exception.Message, Is.EqualTo("Something went wrong"));

            _authService.Verify(
                x => x.SignupAsync(request),
                Times.Once);
        }
    }
}
