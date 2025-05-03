using System;

namespace BackEndGasApp.Attributes
{
    /// <summary>
    /// Attribute to specify which user roles have access to a property.
    /// Properties with this attribute will only be visible to users with at least one of the specified roles.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RoleAccessAttribute : Attribute
    {
        /// <summary>
        /// Gets the roles that are allowed to access the property.
        /// </summary>
        public string[] AllowedRoles { get; }

        /// <summary>
        /// Creates a new instance of RoleAccessAttribute with specified roles.
        /// </summary>
        /// <param name="roles">One or more role names that should have access to the property</param>
        public RoleAccessAttribute(params string[] roles)
        {
            AllowedRoles = roles ?? Array.Empty<string>();
        }
    }

    /// <summary>
    /// Shorthand attribute for Admin-only access to a property.
    /// Equivalent to [RoleAccess("Admin")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AdminOnlyAttribute : RoleAccessAttribute
    {
        /// <summary>
        /// Creates a new instance of AdminOnlyAttribute.
        /// </summary>
        public AdminOnlyAttribute()
            : base("Admin") { }
    }
}
