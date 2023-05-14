using Microsoft.EntityFrameworkCore;

namespace UserService_Core6Api.Models
{
    public class UsersDbContext :DbContext
    {
        public UsersDbContext(DbContextOptions options):base(options)
        {            
        }
        public DbSet<User> Users { get; set; }  
    }
}
