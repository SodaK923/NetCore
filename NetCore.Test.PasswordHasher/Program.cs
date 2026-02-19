using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.Test.PasswordHasher
{
    internal class Program
    {
        // Password => GUIDSalt, RNGSalt, PasswordHash
        static void Main(string[] args)
        {
            Console.WriteLine("아이디를 입력하세요:  ");
            string userId = Console.ReadLine();

            Console.WriteLine("비밀번호를 입력하세요: ");
            string password = Console.ReadLine();

            string guidSalt = Guid.NewGuid().ToString(); // 솔트 생성 방법 1: Guid 사용

            string rngSalt = GetRNGSalt();

            string passwordHash = GetPasswordHash(userId, password, guidSalt, rngSalt);

            // 데이터베이스의 비밀번호 정보와 지금 입력한 비밀번호 정보를 비교해서 같은 해시 값이 나오면 true(로그인 성공), 아니면 false
            bool check = CheckThePasswordInfo(userId, password, guidSalt, rngSalt, passwordHash); 


            Console.WriteLine($"UserID: {userId}");
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"GUIDSalt: {guidSalt}");
            Console.WriteLine($"RNGSalt: {rngSalt}");
            Console.WriteLine($"PasswordHash: {passwordHash}");
            Console.WriteLine($"Check: {(check ? "비밀번호 정보가 일치" : "불일치")}");

            Console.ReadLine();

        }

        private static string GetRNGSalt()
        {
            byte[] salt = new byte[128 / 8]; // 128비트 솔트 생성
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); // 솔트에 랜덤 바이트 채우기
            }

            return Convert.ToBase64String(salt);
        }

        private static string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userId + password + guidSalt,
                salt: Encoding.UTF8.GetBytes(rngSalt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 45000, // 10000, 25000, 45000
                numBytesRequested: 256 / 8)); // 256비트 해시 생성
        }

        private static bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash); // passwordHash는 데이터베이스에 저장된 해시값, GetPasswordHash()는 지금 입력한 비밀번호로 해시값을 계산한 것, 둘이 같으면 true, 아니면 false
        }
    }
}
