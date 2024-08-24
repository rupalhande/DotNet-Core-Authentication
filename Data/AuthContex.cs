using JwtAuth.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Data
{
    public class AuthContex:DbContext
    {
        private readonly IConfiguration _config;
        public AuthContex(DbContextOptions<AuthContex> options,IConfiguration config): base(options)
        {
            this._config = config;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conn = _config["ConnectionStrings:Auth"]?? throw new Exception("Connection string missing");
            string dbName = _config["ConnectionStrings:DbName"]??throw new Exception("DbName Missing");
            string dbUser = _config["ConnectionStrings:DbUserId"]??throw new Exception("Db UserName Missing");
            string dbUserPassword = _config["ConnectionStrings:DbUserPassword"]??throw new Exception("Db User Password Missing");

            string ConnectionStrings = String.Format(conn, dbName, dbUser, dbUserPassword);

            optionsBuilder.UseSqlServer(ConnectionStrings);

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<User> users { get; set; }
        public DbSet<Product> products { get; set; }
    }
}
