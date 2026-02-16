using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.DataModels
{
    // DB 1:1 매핑 데이터
    // 데이터 어노테이션
    public class User
    {
        // 사용자 아이디: PK 지정, 컬럼 길이, 컬럼 타입 지정(숫자는 맞춰야 함)
        [Key, StringLength(50), Column(TypeName = "varchar(50)", Order = 0)]
        public string UserId { get; set; }

        [Required, StringLength(100), Column(TypeName = "nvarchar(100)")]
        public string UserName { get; set; }

        // 중복 안 되는 인덱스 지정
        [Required, StringLength(320), Column(TypeName = "varchar(320)")]
        public string UserEmail { get; set; }

        [Required, StringLength(130), Column(TypeName = "nvarchar(130)")]
        public string Password { get; set; }

        //[Required]
        public bool? IsMembershipWithdrawn { get; set; } // 탈퇴 여부

        [Required]
        public DateTime JoinedUtcDate { get; set; }

        // FK 지정
        [ForeignKey("UserId")]
        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }

    }
}
