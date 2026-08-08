using RestaurantManagement.Filters;
using RestaurantManagement.Services.Configuration;
using System.Web.Http;
using Unity;

namespace RestaurantManagement
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new JwtAuthenticationFilter(UnityConfig.Container.Resolve<IEnvironmentConfigurationService>()));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
