using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public class ChangeInfo
    {
        [Required(ErrorMessage = "사용자 이름을 입력하세요.")]
        [Display(Name = "사용자 이름")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "사용자 이메일을 입력하세요.")]
        [Display(Name = "사용자 이메일")]
        public string UserEmail { get; set; }

        /// <summary>
        /// true: 전부 똑같을 때, false: 하나라도 다를 때
        /// </summary>
        /// <param name="other">비교할 다른 클래스</param>
        /// <returns></returns>
        public bool Equals(UserInfo other)
        {
            if(!string.Equals(UserName, other.UserName, StringComparison.OrdinalIgnoreCase)) // 대소문자를 가리지 않고 비교
            {
                return false;
            }

            if (!string.Equals(UserEmail, other.UserEmail, StringComparison.OrdinalIgnoreCase)) // 대소문자를 가리지 않고 비교
            {
                return false;
            }

            return true;
        }
    }
}
