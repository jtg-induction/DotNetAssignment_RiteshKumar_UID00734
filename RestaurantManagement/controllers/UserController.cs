using Microsoft.IdentityModel.JsonWebTokens;
using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;
using RestaurantManagement.Exceptions;
using RestaurantManagement.Services.Users.Interfaces;
using RestaurantManagement.Filters;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestaurantManagement.Controllers
{
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPatch]
        [JwtAuthorize(Roles = "User,SuperAdmin")]
        [Route("profile")]
        public async Task<IHttpActionResult> PatchProfile(
            PatchProfileRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            long userId = long.Parse(
                ((ClaimsIdentity)User.Identity)
                    .FindFirst(JwtRegisteredClaimNames.Sub)
                    .Value);

            try
            {
                UserProfileResponse response = await _userService.PatchProfileAsync(userId, request);

                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                return Content(HttpStatusCode.NotFound, ex.Message);
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

        [HttpPatch]
        [JwtAuthorize(Roles = "User,SuperAdmin")]
        [Route("deactivate")]
        public async Task<IHttpActionResult> DeactivateAccount()
        {
            try
            {
                long userId = long.Parse(
                    ((ClaimsIdentity)User.Identity)
                        .FindFirst(JwtRegisteredClaimNames.Sub)
                        .Value);

                await _userService.DeactivateAccountAsync(userId);

                MessageResponse response =
                    new MessageResponse
                    {
                        Message = "Account deactivated successfully."
                    };

                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                return Content(
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [JwtAuthorize(Roles = "User,SuperAdmin")]
        [Route("change-password")]
        public async Task<IHttpActionResult> ChangePassword(
            ChangePasswordRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            long userId = long.Parse(
                ((ClaimsIdentity)User.Identity)
                    .FindFirst(JwtRegisteredClaimNames.Sub)
                    .Value);

            try
            {
                await _userService.ChangePasswordAsync(
                    userId,
                    request);


                return Ok(new
                {
                    Message = "Password changed successfully."
                });
            }
            catch (UserNotFoundException ex)
            {
                return Content(
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            catch (InvalidPasswordException ex)
            {
                return Content(
                    HttpStatusCode.BadRequest,
                    ex.Message);
            }
            catch (SamePasswordException ex)
            {
                return Content(
                    HttpStatusCode.BadRequest,
                    ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [JwtAuthorize(Roles = "User,SuperAdmin")]
        [Route("addresses")]
        public async Task<IHttpActionResult> AddAddress(
             AddAddressRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            long userId = long.Parse(
                ((ClaimsIdentity)User.Identity)
                    .FindFirst(JwtRegisteredClaimNames.Sub)
                    .Value);

            try
            {
                UserAddressResponse response =
                    await _userService.AddAddressAsync(
                        userId,
                        request);

                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                return Content(
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            catch (UserInactiveException ex)
            {
                return Content(
                    HttpStatusCode.BadRequest,
                    ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [JwtAuthorize(Roles = "User,SuperAdmin")]
        [Route("addresses/{addressId:long}")]
        public async Task<IHttpActionResult> UpdateAddress(
            long addressId,
            UpdateAddressRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            long userId = long.Parse(
                ((ClaimsIdentity)User.Identity)
                    .FindFirst(JwtRegisteredClaimNames.Sub)
                    .Value);

            try
            {
                UserAddressResponse response =
                    await _userService.UpdateAddressAsync(
                        userId,
                        addressId,
                        request);

                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                return Content(
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            catch (UserInactiveException ex)
            {
                return Content(
                    HttpStatusCode.BadRequest,
                    ex.Message);
            }
            catch (AddressNotFoundException ex)
            {
                return Content(
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}
