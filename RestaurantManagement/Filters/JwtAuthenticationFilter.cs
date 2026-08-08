using Microsoft.IdentityModel.Tokens;
using RestaurantManagement.Services.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RestaurantManagement.Filters
{
    public class JwtAuthenticationFilter : AuthorizationFilterAttribute
    {
        private readonly IEnvironmentConfigurationService _environmentConfigurationService;


        public JwtAuthenticationFilter(
            IEnvironmentConfigurationService environmentConfigurationService)
        {
            _environmentConfigurationService = environmentConfigurationService;
        }


        public override void OnAuthorization(
            HttpActionContext actionContext)
        {
            var authHeader =
                actionContext.Request.Headers.Authorization;


            if (authHeader == null ||
                authHeader.Scheme != "Bearer")
            {
                return;
            }


            string token = authHeader.Parameter;


            try
            {
                JwtSecurityTokenHandler tokenHandler =
                    new JwtSecurityTokenHandler();


                TokenValidationParameters validationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    _environmentConfigurationService.JwtSecret)),


                        ValidateIssuer = false,

                        ValidateAudience = false,


                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero
                    };


                ClaimsPrincipal principal =
                    tokenHandler.ValidateToken(
                        token,
                        validationParameters,
                        out _);


                actionContext.RequestContext.Principal =
                    principal;


                HttpContext.Current.User =
                    principal;
            }
            catch
            {
                // Invalid token.
                // Leave request unauthenticated.
            }
        }
    }
}
