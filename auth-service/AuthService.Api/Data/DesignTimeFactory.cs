using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthService.Api.Data;

public class DesignTimeFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {

        var cs = "Server=localhost,1433;Database=authdb;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True";

        var opts = new DbContextOptionsBuilder<AuthDbContext>()
            .UseSqlServer(cs)
            .Options;

        return new AuthDbContext(opts);
    }
}
