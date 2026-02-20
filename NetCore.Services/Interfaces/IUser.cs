using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Interfaces
{
    // 인터페이스: 실제 사용할 서비스 메서드를 정의하는 곳
    public interface IUser
    {
        bool MatchTheUserInfo(LoginInfo login);
        User GetUserInfo(string userId);
        IEnumerable<UserRolesByUser> GetRolesOwnedByUser(string userId);
        /// <summary>
        /// 사용자 가입
        /// </summary>
        /// <param name="register">사용자 가입용 뷰 모델</param>
        /// <returns></returns>
        int RegisterUser(RegisterInfo register);
    }
}
