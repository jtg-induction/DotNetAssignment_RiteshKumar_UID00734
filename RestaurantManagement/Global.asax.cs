using System.IdentityModel.Tokens.Jwt;
using System.Web.Http;
using Unity.WebApi;

namespace RestaurantManagement
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            GlobalConfiguration.Configuration.DependencyResolver =
                new UnityDependencyResolver(UnityConfig.Container);
        }
    }
}