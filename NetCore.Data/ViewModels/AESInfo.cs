using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public class AESInfo
    {
        [Required(ErrorMessage = "사용자 아이디를 입력하세요.")]
        [MinLength(6, ErrorMessage = "사용자 아이디는 6자 이상 입력하세요.")]
        [Display(Name = "사용자 아이디")]
        public string UserId { get; set; }

        [DataType(DataType.Password)] // 입력한 값이 비밀번호임을 나타냄
        [Required(ErrorMessage = "비밀번호를 입력하세요.")]
        [MinLength(6, ErrorMessage = "비밀번호는 6자 이상 입력하세요.")]
        [Display(Name = "비밀번호")]
        public string Password { get; set; }

        [DataType(DataType.MultilineText)] // 여러 줄 입력이 가능하도록 설정]
        [Display(Name = "암호화 정보")] // 화면에서 보여지는 이름
        public string? EncUserInfo { get; set; } // ****************** ?로 null 허용을 해줘야 함 ****************

        [Display(Name = "복호화 정보")]
        public string? DecUserInfo { get; set; }
    }
}
