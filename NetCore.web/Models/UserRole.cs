using System;
using System.Collections.Generic;

namespace NetCore.web.Models;

public partial class UserRole
{
    public string RoleId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public byte RolePriority { get; set; }

    public DateTime ModifiedUtcDate { get; set; }

    public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; } = new List<UserRolesByUser>();
}
