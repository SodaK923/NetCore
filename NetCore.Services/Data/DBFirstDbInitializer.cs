using NetCore.Data.Classes;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Data
{
    public class DBFirstDbInitializer // 초기데이터 입력기능
    {
        private DBFirstDbContext _context;
        private IPasswordHasher _hasher;
        public DBFirstDbInitializer(DBFirstDbContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        /// <summary>
        /// 초기 데이터를 심는다. (처음 로그인할 계정을 만들어둠)
        /// </summary>
        public int PlantSeedData()
        {
            int rowAffected = 0;
            string userId = "jsootv";
            string password = "123456";
            var utcNow = DateTime.UtcNow;

            var passwordInfo = _hasher.SetPasswordInfo(userId, password);


            _context.Database.EnsureCreated(); // DB 자동생성(그래도 미리 만들 것을 권장)

            // 사용자 생성
            if (!_context.Users.Any()) // 데이터가 비어있을 때만
            {
                var users = new List<User>()
                {
                    new User()
                    {
                        UserId = userId.ToLower(),
                        UserName = "Seed 사용자",
                        UserEmail = "jsootv@gmail.com",
                        GUIDSalt = passwordInfo.GUIDSalt,
                        RNGSalt = passwordInfo.RNGSalt,
                        PasswordHash = passwordInfo.PasswordHash,
                        AccessFailedCount = 0,
                        IsMembershipWithdrawn = false,
                        JoinedUtcDate = utcNow
                    }
                };

                _context.Users.AddRange(users);
                rowAffected = _context.SaveChanges();
            }

            // 권한 등급 설정
            if (!_context.UserRoles.Any())
            {
                var userRoles = new List<UserRole>()
                {
                    new UserRole()
                    {
                        RoleId = "AssociateUser",
                        RoleName = "준사용자",
                        RolePriority = 1,
                        ModifiedUtcDate = utcNow
                    },

                    new UserRole()
                    {
                        RoleId = "GeneralUser",
                        RoleName = "일반사용자",
                        RolePriority = 2,
                        ModifiedUtcDate = utcNow
                    },

                    new UserRole()
                    {
                        RoleId = "SuperUser",
                        RoleName = "향상된 사용자",
                        RolePriority = 3,
                        ModifiedUtcDate = utcNow
                    },

                    new UserRole()
                    {
                        RoleId = "SystemUser",
                        RoleName = "시스템 사용자",
                        RolePriority = 4,
                        ModifiedUtcDate = utcNow
                    }
                };

                _context.UserRoles.AddRange(userRoles);
               rowAffected += _context.SaveChanges();
            }


            // 권한 부여
            if (!_context.UserRolesByUsers.Any())
            {
                var userRolesByUsers = new List<UserRolesByUser>()
                {
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "GeneralUser",
                        OwnedUtcDate = utcNow
                    },

                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SuperUser",
                        OwnedUtcDate = utcNow
                    },

                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SystemUser",
                        OwnedUtcDate = utcNow
                    },
                };

                _context.UserRolesByUsers.AddRange(userRolesByUsers);
                rowAffected += _context.SaveChanges();
            }

            return rowAffected;
        }
    }
}
