using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace FoodSphere.Common.Constant;

public enum UserType
{
    Admin,
    Master,
    Consumer,
    Staff,
}

public static class FoodSphereClaimType
{
    public const string UserTypeClaimType = "user_type";
    public const string BillClaimType = "bill_id";
    public const string BillMemberClaimType = "bill_member_id";
    public const string RestaurantClaimType = "restaurant_id";
    public const string BranchClaimType = "branch_id";

    public static readonly ClaimsIdentityOptions Identity = new()
    {
        RoleClaimType = "role",
        UserNameClaimType = "username",
        UserIdClaimType = "sub",
        EmailClaimType = "email",
        SecurityStampClaimType = "stamp"
    };

    // Cast<T>() => throw invalid types
    // OfType<T>() => skip (filters)
    public static IEnumerable<string> GetAll() =>
        typeof(FoodSphereClaimType)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => f.GetValue(null))
            .Cast<string>();
}