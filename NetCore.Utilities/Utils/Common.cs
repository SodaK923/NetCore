using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;

namespace NetCore.Utilities.Utils
{
    public static class Common
    {
        public static void SetDataProtection(IServiceCollection services, string keyPath, string applicationName, Enum cryptoType)
        {
            // Program.cs에서 builder.Services.AddDataProtection()로 시작하는 부분을 이 메서드로 옮겨서 사용
            var builder = services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                .SetApplicationName(applicationName);

            switch (cryptoType)
            {
                case Enums.cryptoType.Unmanaged:
                    // AES
                    // Advanced Encryption Standard
                    // Two-way: 암호화, 복호화
                    builder.UseCryptographicAlgorithms(
                        new Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.EncryptionAlgorithm.AES_256_CBC,

                            // SHA
                            // Secure Hash Algorithm
                            // One-way: 암호화
                            ValidationAlgorithm = Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ValidationAlgorithm.HMACSHA512
                        });
                    break;

                case Enums.cryptoType.Managed:
                    builder.UseCustomCryptographicAlgorithms(
                        new ManagedAuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithmType = typeof(System.Security.Cryptography.Aes),
                            EncryptionAlgorithmKeySize = 256,
                            ValidationAlgorithmType = typeof(HMACSHA512)
                        });
                    break;

                case Enums.cryptoType.CngCbc:
                    // Windows CNG (Cryptography Next Generation) API를 사용하여 CBC 모드로 암호화
                    // CBC (Cipher Block Chaining) 모드는 블록 암호화 방식 중 하나로,
                    // 각 블록이 이전 블록의 암호문과 XOR 연산을 수행하여 암호화되는 방식입니다.
                    // 이를 통해 동일한 평문이 여러 번 암호화되더라도 서로 다른 암호문이 생성되어 보안성이 향상됩니다.
                    builder.UseCustomCryptographicAlgorithms(
                        new CngCbcAuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,
                            EncryptionAlgorithmKeySize = 256,
                            HashAlgorithm = "SHA512",
                            HashAlgorithmProvider = null
                        });
                    break;

                case Enums.cryptoType.CngGcm:
                    // Windows CNG (Cryptography Next Generation) API를 사용하여 GCM 모드로 암호화
                    // GCM (Galois/Counter Mode) 모드는 블록 암호화 방식 중 하나로,
                    // 암호화와 인증을 동시에 수행하는 방식입니다. GCM 모드는 데이터의 무결성과 기밀성을 보장하며,
                    // 특히 대량의 데이터를 암호화할 때 효율적입니다. GCM 모드는 인증 태그를 생성하여 데이터의 무결성을 검증할 수 있도록 합니다.
                    builder.UseCustomCryptographicAlgorithms(
                        new CngGcmAuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,
                            EncryptionAlgorithmKeySize = 256,
                        });
                    break;
            }
        }
    }
}
