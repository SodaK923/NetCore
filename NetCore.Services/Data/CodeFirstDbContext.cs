using Microsoft.EntityFrameworkCore;
using NetCore.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Data
{
    // 2. Fulent API 방식 - Code First
    // Entity → (무엇을 설정할 건지 선택) → (그것의 옵션 설정)


    // 상속
    // CodeFirstDbContext: 자식 클래스
    // DbContext: 부모 클래스, Entity Framework Core에서 데이터베이스와 상호작용하기 위한 기본 클래스
    public class CodeFirstDbContext : DbContext
    {
        // 생성자 상속: 부모 클래스의 생성자를 호출하여 초기화
        public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options) : base(options)
        {
            
        }

        // DB 테이블 리스트 지정
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRolesByUser> UserRoleByUsers { get; set; }

        // 메서드 상속: 부모 클래스의 메서드를 재정의하여 추가 기능 구현
        // 부모 클래스에서 OnModelCreating 메서드가 가상 메서드로 정의되어 있어 재정의 가능
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 4가지 작업
            // DB 테이블이름 변경
            modelBuilder.Entity<User>().ToTable(name: "User");
            modelBuilder.Entity<UserRole>().ToTable(name: "UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable(name: "UserRolesByUser");

            // 복합키 지정(두 개 이상의 컬럼을 기본키로 설정)
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });

            // 컬럼 기본값 지정
            //modelBuilder.Entity<User>(e =>
            //{
            //    e.Property(c => c.IsMembershipWithdrawn).HasDefaultValue(value: false);
            //});
            modelBuilder.Entity<User>().Property(c => c.IsMembershipWithdrawn).HasDefaultValue(value: false);
               

            // 인덱스 지정: 중복이 되지 않도록
            modelBuilder.Entity<User>().HasIndex(c => c.UserEmail ).IsUnique(unique: true);


            // User거 이렇게 다 합쳐도 됨 (e는 User 엔티티를 나타내는 매개변수)
            //modelBuilder.Entity<User>(e =>
            //{
            //    e.ToTable(name: "User");
            //    e.HasIndex(c => new { c.UserEmail }).IsUnique(unique: true);
            //    e.Property(c => c.IsMembershipWithdrawn).HasDefaultValue(value: false);
            //});
        }
    }
}
