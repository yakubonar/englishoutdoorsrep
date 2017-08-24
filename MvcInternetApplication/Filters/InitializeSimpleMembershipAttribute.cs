using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using MvcInternetApplication.Models;
using System.Web.Security;

namespace MvcInternetApplication.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Инициализация ASP.NET Simple Membership происходит один раз при старте приложения
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<UsersContext>(null);

                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }
                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

                    SimpleRoleProvider roles = (SimpleRoleProvider)Roles.Provider;
                    SimpleMembershipProvider membership = (SimpleMembershipProvider)Membership.Provider;

                    // Проверка наличия роли Admin
                    if (!roles.RoleExists("SomeRole"))
                    {
                        roles.CreateRole("SomeRole");
                    }

                    // Поиск пользователя с логином admin
                    if (membership.GetUser("SomeName", false) == null)
                    {
                        membership.CreateUserAndAccount("SomeName", "SomePassword"); // создание пользователя
                        roles.AddUsersToRoles(new[] { "SomeName" }, new[] { "SomeRole" }); // установка роли для пользователя
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}



