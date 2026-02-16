using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Data
{
    public class DBFirstDbContext : DbContext
    {
        public DBFirstDbContext(DbContextOptions<DBFirstDbContext> options) : base(options)
        {

        }

        // DB 테이블 리스트 지정
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserRole> UserRoles { get; set; }

        public virtual DbSet<UserRolesByUser> UserRolesByUsers { get; set; }

        // virtual
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // DB First 방식에서는 데이터베이스에서 테이블과 컬럼 정보를 가져와서 모델을 자동으로 생성하기 때문에
            // 별도의 모델 설정이 필요하지 않습니다.

            // DB 테이블이름 변경
            modelBuilder.Entity<User>().ToTable(name: "User");
            modelBuilder.Entity<UserRole>().ToTable(name: "UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable(name: "UserRolesByUser");

            // 복합키 지정
            modelBuilder.Entity<UserRolesByUser>().HasKey(c=> new {c.UserId, c.RoleId});
        }
    }
}
