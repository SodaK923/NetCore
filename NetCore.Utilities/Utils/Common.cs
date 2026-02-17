using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
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
                    break;

                case Enums.cryptoType.Managed:
                    break;

                case Enums.cryptoType.CngCbc:
                    break;

                case Enums.cryptoType.CngGcm:
                    break;
            }
        }
    }
}
