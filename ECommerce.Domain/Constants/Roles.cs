using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string SuperAdmin = "SuperAdmin";
    public const string User = "User";

    public static readonly string[] AllRoles = new[] { 
        Admin,
        SuperAdmin,
        User };

}
