using RestaurantManagement.Data;
using RestaurantManagement.Repository;
using RestaurantManagement.Repository.Interfaces;
using RestaurantManagement.Services.Auth;
using RestaurantManagement.Services.Configuration;
using RestaurantManagement.Services.Users;
using RestaurantManagement.Services.Users.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using Unity;
using Unity.Lifetime;

namespace RestaurantManagement
{
    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container =
            new Lazy<IUnityContainer>(() =>
            {
                var container = new UnityContainer();

                RegisterTypes(container);

                return container;
            });

        public static IUnityContainer Container => container.Value;


        public static void RegisterTypes(IUnityContainer container)
        {
            // JWT Handler 
            container.RegisterType<JwtSecurityTokenHandler>(
                new ContainerControlledLifetimeManager()
            );

            // Database Context 
            container.RegisterType<RestaurantDbContext>(
                new HierarchicalLifetimeManager()
            );

            // Repositories
            container.RegisterType<IUserRepository, UserRepository>(
                new HierarchicalLifetimeManager()
            );

            container.RegisterType<IRefreshTokenRepository, RefreshTokenRepository>(
                new HierarchicalLifetimeManager()
            );


            // Application Services 
            container.RegisterType<IAuthService, AuthService>(
                new HierarchicalLifetimeManager()
            );

            container.RegisterType<IUserService, UserService>(
                new HierarchicalLifetimeManager()
            );

            // Stateless Services 
            container.RegisterType<IPasswordHasher, PasswordHasher>(
                new ContainerControlledLifetimeManager()
            );

            container.RegisterType<IEnvironmentConfigurationService, EnvironmentConfigurationService>(
                 new ContainerControlledLifetimeManager()
            );

            container.RegisterType<IJwtService, JwtService>(
                new ContainerControlledLifetimeManager()
            );

            container.RegisterType<IRefreshTokenService, RefreshTokenService>(
                new ContainerControlledLifetimeManager()
            );
        }
    }
}
