using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.DataModels
{
    public class UserRolesByUser
    {
        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public string UserId { get; set; }

        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public string RoleId { get; set; }

        [Required]
        public DateTime OwnedUtcDate { get; set; }


        public virtual User User { get; set; }

        public virtual UserRole Role { get; set; } // 타입이랑 이름이 겹쳐서 UserRole -> Role로 수정
    }
}
