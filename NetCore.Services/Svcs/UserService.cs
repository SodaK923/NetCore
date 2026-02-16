//using NetCore.Data.DataModels;
using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {
        private DBFirstDbContext _context;

        public UserService(DBFirstDbContext context)
        {
            _context = context;
        }
        // User 테이블에서 사용자 정보를 받아오는 메서드
        #region private methods
        private IEnumerable<User> GetUserInfos()
        {
            return _context.Users.ToList();
            //return new List<User>()
            //{
            //    new User()
            //    {
            //        UserId = "jadejs",
            //        UserName = "김정수",
            //        UserEmail = "jadejskim@gmail.com",
            //        Password = "123456"

            //    }
            //};
        }

        private bool CheckTheUserInfo(string userId, string password)
        {
            // LINQ: GetUserInfos()에서 사용자 정보를 받아와서, 입력받은 userId와 password가 일치하는 사용자가 있는지 확인
            // Any(): 조건에 맞는 요소가 하나라도 있는지 확인하는 메서드
            return GetUserInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any();
        }

        #endregion

        bool IUser.MatchTheUserInfo(LoginInfo login)
        {
            return CheckTheUserInfo(login.UserId, login.Password);
        }
    }

}
