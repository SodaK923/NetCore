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
    }
}
