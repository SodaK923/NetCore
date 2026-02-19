using System.ComponentModel.DataAnnotations;

namespace NetCore.Data.ViewModels
{
    // 화면 전용 모델(로그인 화면에서 필요한 것만 있음
    public class LoginInfo
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

        [Display(Name = "내 정보 기억")]
        public bool RememberMe { get; set; }
        }
}
