using System;
using System.Collections.Generic;

namespace NetCore.web.Models;

public partial class UserRolesByUser
{
    public string UserId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public DateTime OwnedUtcDate { get; set; }

    public virtual UserRole Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
