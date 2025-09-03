using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MonAPI.Data;

public class MaBaseDbContext : IdentityDbContext<ApplicationUser>
{

    public MaBaseDbContext(DbContextOptions<MaBaseDbContext> options) : base(options)
    {

    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AuthorizedApplication> AuthorizedApplications { get; set; }

}
