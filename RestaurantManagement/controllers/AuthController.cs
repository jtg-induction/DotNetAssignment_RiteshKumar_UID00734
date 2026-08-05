using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;
using RestaurantManagement.Exceptions;
using RestaurantManagement.Services.Auth;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantManagement.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> Signup(SignupRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                AuthResponse response = await _authService.SignupAsync(request);
                return Ok(response);
            }

            catch (EmailAlreadyExistsException ex)
            {
                return Content(HttpStatusCode.Conflict, ex.Message);
            }

            catch (PhoneAlreadyExistsException ex)
            {
                return Content(HttpStatusCode.Conflict, ex.Message);
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
