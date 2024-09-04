using Microsoft.EntityFrameworkCore;
using BasicDotnetTemplate.MainProject.Models.Database.SqlServer;


namespace BasicDotnetTemplate.MainProject.Core.Database
{
    public class SqlServerContext : DbContext
    {

        public SqlServerContext(DbContextOptions<SqlServerContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}



