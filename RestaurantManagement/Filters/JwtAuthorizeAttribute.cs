using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace RestaurantManagement.Filters
{
    public class JwtAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public string Roles { get; set; }


        public override void OnAuthorization(
            System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var principal =
                actionContext.RequestContext.Principal;


            if (principal == null ||
                principal.Identity == null ||
                !principal.Identity.IsAuthenticated)
            {
                actionContext.Response =
                    actionContext.Request
                    .CreateResponse(
                        HttpStatusCode.Unauthorized);

                return;
            }


            if (!string.IsNullOrWhiteSpace(Roles))
            {
                string[] allowedRoles =
                    Roles.Split(',')
                    .Select(x => x.Trim())
                    .ToArray();


                bool hasRole =
                    allowedRoles.Any(role =>
                        principal.IsInRole(role));


                if (!hasRole)
                {
                    actionContext.Response =
                        actionContext.Request
                        .CreateResponse(
                            HttpStatusCode.Forbidden);
                }
            }
        }
    }
}
