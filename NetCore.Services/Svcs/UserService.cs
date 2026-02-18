//using NetCore.Data.DataModels;
using Microsoft.EntityFrameworkCore;
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

        private User GetUserInfo(string userId, string password)
        {
            User user;

            // 람다(권장)
            user = _context.Users.Where(u=>u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();

            // FromSql(Table, View, Function, Stored Procedure)

            // Table
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate FROM dbo.[User]")
            //    .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //    .FirstOrDefault();

            // view
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate FROM dbo.[uvwUser]")
            //    .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //    .FirstOrDefault();

            // function -> 안됨
            //user = _context.Users.FromSqlRaw($"SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate FROM dbo.ufnUser({userId}, {password})")
            //    .FirstOrDefault();

            //user = _context.Users
            //    .FromSqlInterpolated($"SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate FROM dbo.ufnUser")
            //    .FirstOrDefault();


            // stored procedure(권장) -> 안됨
            //user = _context.Users.FromSqlInterpolated($"EXEC dbo.uspCheckLoginByUserId {userId} , {password}") // int 식이 들어가면 ToString()을 써줘야됨
            //    .FirstOrDefault();

            if (user == null)
            {
                // 접속 실패 횟수에 대한 증가
                int rowAffected;

                // SQL문 직접 작성
                // rowAffected = _context.Database.ExecuteSqlInterpolated($"Update dbo.[User] SET AccessFailedCount += 1 WHERE UserId = {userId}");

                // Stored Procedure 사용
                //rowAffected = _context.Database.ExecuteSqlInterpolated($"dbo.FailedLoginByUserId @p0", new[] {userId});
            }

            return user;
        }

        private bool CheckTheUserInfo(string userId, string password)
        {
            // LINQ: GetUserInfos()에서 사용자 정보를 받아와서, 입력받은 userId와 password가 일치하는 사용자가 있는지 확인
            // Any(): 조건에 맞는 요소가 하나라도 있는지 확인하는 메서드
            // return GetUserInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any();
            return GetUserInfo(userId, password) != null ? true : false;
        }

        #endregion

        bool IUser.MatchTheUserInfo(LoginInfo login)
        {
            return CheckTheUserInfo(login.UserId, login.Password);
        }
    }

}
