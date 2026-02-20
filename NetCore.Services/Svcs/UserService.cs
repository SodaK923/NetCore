//using NetCore.Data.DataModels;
using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
//using NetCore.Data.DataModels;
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
        private IPasswordHasher _hasher;

        public UserService(DBFirstDbContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
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
            //user = _context.Users.Where(u=>u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();
            //user = _context.Users.Where(u => u.UserId.Equals(userId)).FirstOrDefault(); // IPasswordHasher 거 써가지고 임시로 바꿈

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
            user = _context.Users.FromSql($"EXEC dbo.uspCheckLoginByUserId {userId}, {password}") // int 식이 들어가면 ToString()을 써줘야됨
                .FirstOrDefault();

            if (user == null)
            {
                // 접속 실패 횟수에 대한 증가
                int rowAffected;

                // SQL문 직접 작성
                // rowAffected = _context.Database.ExecuteSqlInterpolated($"Update dbo.[User] SET AccessFailedCount += 1 WHERE UserId = {userId}");

                // Stored Procedure 사용
                rowAffected = _context.Database.ExecuteSql($"EXEC dbo.FailedLoginByUserId {userId}");
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

        private User GetUserInfo(string userId)
        {
            return _context.Users.Where(u => u.UserId.Equals(userId)).FirstOrDefault();
        }

        private IEnumerable<UserRolesByUser> GetUserRolesByUserInfos(string userId)
        {
            var userRolesByUserInfos = _context.UserRolesByUsers.Where(uru => uru.UserId.Equals(userId)).ToList();

            foreach(var role in userRolesByUserInfos)
            {
                role.Role = GetUserRole(role.RoleId);
            }

            return userRolesByUserInfos.OrderByDescending(uru => uru.Role.RolePriority);
        }

        private UserRole GetUserRole(string roleId)
        {
            return _context.UserRoles.Where(ur => ur.RoleId.Equals(roleId)).FirstOrDefault();
        }

        // 아이디에 대해서 대소문자 처리(ToLower())
        private int RegisterUser(RegisterInfo register)
        {
            var utcNow = DateTime.UtcNow;
            var passwordInfo = _hasher.SetPasswordInfo(register.UserId, register.Password);

            var user = new User()
            {
                UserId = register.UserId.ToLower(),
                UserName = register.UserName,
                UserEmail = register.UserEmail,
                GUIDSalt = passwordInfo.GUIDSalt,
                RNGSalt = passwordInfo.RNGSalt,
                PasswordHash = passwordInfo.PasswordHash,
                AccessFailedCount = 0,
                IsMembershipWithdrawn = false,
                JoinedUtcDate = utcNow
            };

            var userRolesByUser = new UserRolesByUser()
            {
                UserId = register.UserId.ToLower(),
                RoleId = "AssociateUser",
                OwnedUtcDate = utcNow
            };

            _context.Add(user);
            _context.Add(userRolesByUser);

            return _context.SaveChanges();
        }

        #endregion

        bool IUser.MatchTheUserInfo(LoginInfo login)
        {
            //return CheckTheUserInfo(login.UserId, login.Password);

            var user = _context.Users.Where(u => u.UserId.Equals(login.UserId)).FirstOrDefault();

            if(user == null)
            {
                return false;
            }

            return _hasher.CheckThePasswordInfo(login.UserId, login.Password, user.GUIDSalt, user.RNGSalt, user.PasswordHash);
        }

        User IUser.GetUserInfo(string userId)
        {
            return GetUserInfo(userId); // GetUserInfo 메서드 생성 -> 인터페이스에서 정의 -> 서비스에서 명시적 구현
        }

        public IEnumerable<UserRolesByUser> GetRolesOwnedByUser(string userId)
        {
            return GetUserRolesByUserInfos(userId);
        }

        int IUser.RegisterUser(RegisterInfo register)
        {
            return RegisterUser(register);
        }
    }

}
