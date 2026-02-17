using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;

namespace NetCore.web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"C\DataProtector\"))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                .SetApplicationName("NetCore");

            // 의존성 주입을 사용하기 위해서 서비스로 등록
            // 껍데기             내용물
            // IUser 인터페이스에 UserService 클래스 인스턴스 주입
            builder.Services.AddScoped<IUser, UserService>();

            // DB 접속 정보, Migrations 프로젝트 지정
            //builder.Services.AddDbContext<CodeFirstDbContext>(options =>
            //options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString(name:"DefaultConnection"),
            //sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations")));

            // DB 접속 정보만
            builder.Services.AddDbContext<DBFirstDbContext>(options =>
            options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString(name: "DBFirstDBConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
