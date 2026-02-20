using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCore.Services.Bridges;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Svcs
{
    public class PasswordHasher : IPasswordHasher
    {
        private DBFirstDbContext _context;

        public PasswordHasher(DBFirstDbContext context)
        {
            _context = context;
        }

        #region private methods
        private string GetGuidSalt()
        {
            return Guid.NewGuid().ToString(); // GUID 솔트 생성
        }

        private string GetRNGSalt()
        {
            byte[] salt = new byte[128 / 8]; // 128비트 솔트 생성
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); // 솔트에 랜덤 바이트 채우기
            }

            return Convert.ToBase64String(salt);
        }

        // 아이디와 비밀번호에 대해서 대소문자 처리(ToLower())
        private string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userId.ToLower() + password.ToLower() + guidSalt,
                salt: Encoding.UTF8.GetBytes(rngSalt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 45000, // 10000, 25000, 45000
                numBytesRequested: 256 / 8)); // 256비트 해시 생성
        }

        private bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash); // passwordHash는 데이터베이스에 저장된 해시값, GetPasswordHash()는 지금 입력한 비밀번호로 해시값을 계산한 것, 둘이 같으면 true, 아니면 false
        }

        private PasswordHashInfo PasswordInfo(string userId, string password)
        {
            string guidSalt = GetGuidSalt();
            string rngSalt = GetRNGSalt();
            var passwordInfo = new PasswordHashInfo()
            {
                GUIDSalt = guidSalt,
                RNGSalt = rngSalt,
                PasswordHash = GetPasswordHash(userId, password, guidSalt, rngSalt)
            };

            return passwordInfo;
        }


        #endregion

        string IPasswordHasher.GetGuidSalt()
        {
            return GetGuidSalt();
        }

        string IPasswordHasher.GetRNGSalt()
        {
            return GetRNGSalt();
        }

        string IPasswordHasher.GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt);
        }

        

        bool IPasswordHasher.CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            return CheckThePasswordInfo(userId, password, guidSalt, rngSalt, passwordHash);
        }

        PasswordHashInfo IPasswordHasher.SetPasswordInfo(string userId, string password)
        {
            return PasswordInfo(userId, password);
        }
    }
}
