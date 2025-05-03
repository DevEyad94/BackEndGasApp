using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using BackEndGasApp.Attributes;
using Microsoft.AspNetCore.Http;

namespace BackEndGasApp.Extensions
{
    /// <summary>
    /// Extension methods for handling role-based access to properties
    /// </summary>
    public static class RoleBasedAccessExtensions
    {
        /// <summary>
        /// Removes access to properties based on RoleAccess attributes and the current user's roles
        /// </summary>
        /// <typeparam name="T">Type of the object to apply access restrictions to</typeparam>
        /// <param name="entity">The object to apply access restrictions to</param>
        /// <param name="httpContextAccessor">HTTP context accessor to get the current user</param>
        /// <returns>The same object with restricted properties set to null</returns>
        public static T ApplyRoleBasedAccess<T>(
            this T entity,
            IHttpContextAccessor httpContextAccessor
        )
            where T : class
        {
            if (entity == null)
                return null;

            var currentUser = httpContextAccessor?.HttpContext?.User;

            // Process each property marked with RoleAccess attribute
            foreach (var prop in typeof(T).GetProperties())
            {
                var attr = prop.GetCustomAttribute<RoleAccessAttribute>();
                if (attr == null)
                    continue;

                // If no user or user doesn't have required roles, set property to null
                bool hasAccess = false;
                if (currentUser != null)
                {
                    hasAccess = attr.AllowedRoles.Any(role => currentUser.IsInRole(role));
                }

                if (!hasAccess && prop.CanWrite && prop.PropertyType == typeof(string))
                {
                    prop.SetValue(entity, null);
                }
            }

            return entity;
        }

        /// <summary>
        /// Checks if the current user has any of the specified roles
        /// </summary>
        /// <param name="httpContextAccessor">HTTP context accessor to get the current user</param>
        /// <param name="roles">Roles to check for</param>
        /// <returns>True if the user has any of the specified roles, false otherwise</returns>
        public static bool HasAnyRole(
            this IHttpContextAccessor httpContextAccessor,
            params string[] roles
        )
        {
            var currentUser = httpContextAccessor?.HttpContext?.User;
            if (currentUser == null || roles == null || roles.Length == 0)
                return false;

            return roles.Any(role => currentUser.IsInRole(role));
        }

        /// <summary>
        /// Checks if the current user has either Admin or User role
        /// </summary>
        /// <param name="httpContextAccessor">HTTP context accessor to get the current user</param>
        /// <returns>True if the user has Admin or User role, false otherwise</returns>
        public static bool IsAdminOrUser(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HasAnyRole("Admin", "User");
        }
    }
}
